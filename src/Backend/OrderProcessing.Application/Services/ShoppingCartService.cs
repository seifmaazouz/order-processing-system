using OrderProcessing.Application.DTOs.ShoppingCart;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Application.Mappings;
using OrderProcessing.Application.Exceptions;
using OrderProcessing.Domain.Exceptions;
using OrderProcessing.Domain.Interfaces.Repositories;
using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.ValueObjects;

namespace OrderProcessing.Application.Services;

public class ShoppingCartService : IShoppingCartService
{
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly IBookRepository _bookRepository;
    private readonly ICreditCardRepository _creditCardRepository;
    private readonly ICustomerOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;

    public ShoppingCartService(
        IShoppingCartRepository shoppingCartRepository, 
        IBookRepository bookRepository,
        ICreditCardRepository creditCardRepository,
        ICustomerOrderRepository orderRepository,
        IUserRepository userRepository)
    {
        _shoppingCartRepository = shoppingCartRepository;
        _bookRepository = bookRepository;
        _creditCardRepository = creditCardRepository;
        _orderRepository = orderRepository;
        _userRepository = userRepository;
    }

    public async Task<ShoppingCartDetailsDto> GetCartDetailsAsync(string username)
    {
        // Get or create cart for user (ensures exactly one cart per user)
        var cart = await _shoppingCartRepository.GetOrCreateCartAsync(username);
        
        // Get all ISBNs at once to avoid N+1 queries
        var isbns = cart.CartItems.Select(i => i.ISBN).ToList();
        var books = new Dictionary<string, (string Title, List<string> Authors, int Stock)>();

        foreach (var isbn in isbns)
        {
            var book = await _bookRepository.GetBookDetailsAsync(isbn);
            if (book != null)
            {
                var authors = (book.AuthorNames ?? "")
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .ToList();
                books[isbn] = (book.Title, authors, book.Quantity);
            }
        }

        var itemsDto = cart.CartItems
            .OrderBy(item => item.ISBN) // Maintain consistent ordering
            .Select(item =>
            {
                var (title, authors, stock) = books.GetValueOrDefault(item.ISBN, (string.Empty, new List<string>(), 0));
                return item.ToCartItemDetailsDto(title, authors, stock);
            }).ToList();
        
        return cart.ToShoppingCartDetailsDto(itemsDto);
    }

    public async Task AddItemToCartAsync(string username, string isbn)
    {
        int quantity = 1; // Always add 1 item

        if (quantity <= 0)
            throw new BusinessRuleViolationException("Quantity must be greater than 0");

        // Fetch book to get price and validate it exists
        var book = await _bookRepository.GetBookDetailsAsync(isbn);
        if (book == null)
            throw new NotFoundException("Book", "ISBN", isbn);

        // Check if book is available (in stock)
        if (book.Quantity <= 0)
            throw new InsufficientStockException(isbn, book.Quantity, book.Title);

        // Get or create cart for user (ensures exactly one cart per user)
        var cart = await _shoppingCartRepository.GetOrCreateCartAsync(username);

        // Check if item already exists in cart and validate total quantity
        var existingItem = cart.CartItems.FirstOrDefault(i => i.ISBN == isbn);
        int newTotalQuantity = existingItem != null ? existingItem.Quantity + quantity : quantity;

        if (newTotalQuantity > book.Quantity)
            throw new InsufficientStockException(isbn, book.Quantity, book.Title);

        var cartItem = new CartItem(0, isbn, quantity, book.SellingPrice);

        try
        {
            await _shoppingCartRepository.AddCartItemAsync(cart.CartId, cartItem);
        }
        catch (Exception ex)
        {
            throw new BusinessRuleViolationException($"Failed to add item to cart: {ex.Message}");
        }
    }

    public async Task UpdateCartItemAsync(string username, string isbn, int quantity)
    {
        if (quantity <= 0)
            throw new BusinessRuleViolationException("Quantity must be greater than 0");

        var cart = await _shoppingCartRepository.GetCartByUsernameAsync(username);
        if (cart == null)
        {
            throw new NotFoundException("Shopping cart", "username", username);
        }

        // Validate stock availability
        var book = await _bookRepository.GetBookDetailsAsync(isbn);
        if (book == null)
            throw new NotFoundException("Book", "ISBN", isbn);

        if (quantity > book.Quantity)
            throw new InsufficientStockException(isbn, book.Quantity, book.Title);

        // Find existing cart item to get current unit price
        var existingItem = cart.CartItems.FirstOrDefault(i => i.ISBN == isbn);
        if (existingItem == null)
            throw new NotFoundException("Cart item", "ISBN", isbn);

            // Repository UpdateCartItemAsync expects a read-model projection; construct and pass that
            var cartItemRead = new Domain.Models.CartItemReadModel(isbn, quantity, existingItem.UnitPrice);
            var affected = await _shoppingCartRepository.UpdateCartItemAsync(cart.CartId, cartItemRead);
        if (affected == 0)
        {
            throw new NotFoundException("Cart item", "ISBN", isbn);
        }
    }

    public async Task RemoveItemFromCartAsync(string username, string isbn)
    {
        var cart = await _shoppingCartRepository.GetCartByUsernameAsync(username);
        if (cart == null)
        {
            throw new NotFoundException("Shopping cart", "username", username);
        }

        await _shoppingCartRepository.RemoveCartItemAsync(cart.CartId, isbn);
    }

    public async Task ClearCartAsync(string username)
    {
        var cart = await _shoppingCartRepository.GetCartByUsernameAsync(username);
        if (cart == null) return;

        await _shoppingCartRepository.ClearCartAsync(cart.CartId);
    }

    public async Task<int> CheckoutAsync(string username, CheckoutDto checkoutDto)
    {
        // Determine payment method
        bool useSaved = checkoutDto.SavedCardNumber.HasValue;
        bool useNew = checkoutDto.NewCardNumber.HasValue || !string.IsNullOrWhiteSpace(checkoutDto.NewCardExpiryDate) || !string.IsNullOrWhiteSpace(checkoutDto.CardholderName);

        if (!useSaved && !useNew)
            throw new BusinessRuleViolationException("You must select a saved card or enter new card details.");
        if (useSaved && useNew)
            throw new BusinessRuleViolationException("Cannot use both saved card and new card. Choose one payment method.");

        // Validate cardholder name for new card
        if (useNew && string.IsNullOrWhiteSpace(checkoutDto.CardholderName))
            throw new BusinessRuleViolationException("Cardholder name is required for new card.");

        var cart = await _shoppingCartRepository.GetCartByUsernameAsync(username);
        if (cart == null || cart.CartItems.Count == 0)
            throw new BusinessRuleViolationException("Cannot checkout an empty cart");

        // Get shipping address
        string? shippingAddress = checkoutDto.ShippingAddress;
        if (string.IsNullOrWhiteSpace(shippingAddress))
        {
            var user = await _userRepository.GetByUserNameAsync(username);
            if (user == null)
                throw new NotFoundException("User", "username", username);
            shippingAddress = user.Address;
        }
        if (string.IsNullOrWhiteSpace(shippingAddress))
            throw new BusinessRuleViolationException("Shipping address is required. Please update your profile with a shipping address.");

        // Get card details
        long cardNumber;
        DateTime expiryDateTime;
        if (useSaved)
        {
            var userCards = await _creditCardRepository.GetUserCardsAsync(username);
            var savedCard = userCards.FirstOrDefault(c => c.CardNumber == checkoutDto.SavedCardNumber);
            if (savedCard == null)
                throw new BusinessRuleViolationException("Saved card not found or does not belong to this user.");
            cardNumber = savedCard.CardNumber;
            expiryDateTime = savedCard.ExpiryDate.ToDateTime(TimeOnly.MinValue);
        }
        else // useNew
        {
            if (!checkoutDto.NewCardNumber.HasValue)
                throw new BusinessRuleViolationException("New card number is required.");
            cardNumber = checkoutDto.NewCardNumber.Value;
            if (string.IsNullOrWhiteSpace(checkoutDto.NewCardExpiryDate))
                throw new BusinessRuleViolationException("New card expiry date is required.");
            if (!DateTime.TryParse(checkoutDto.NewCardExpiryDate, out expiryDateTime))
            {
                if (checkoutDto.NewCardExpiryDate.Contains('T'))
                {
                    var datePart = checkoutDto.NewCardExpiryDate.Split('T')[0];
                    if (!DateTime.TryParse(datePart, out expiryDateTime))
                        throw new BusinessRuleViolationException($"Invalid expiry date format: {checkoutDto.NewCardExpiryDate}. Expected YYYY-MM-DD format.");
                }
                else
                {
                    throw new BusinessRuleViolationException($"Invalid expiry date format: {checkoutDto.NewCardExpiryDate}. Expected YYYY-MM-DD format.");
                }
            }
        }

        // Validate credit card
        if (useSaved)
        {
            // For saved cards, validate they exist in database and are not expired
            var isValidCard = await _creditCardRepository.ValidateCreditCardAsync(cardNumber, expiryDateTime);
            if (!isValidCard) throw new BusinessRuleViolationException("Saved card is invalid or expired");
        }
        else
        {
            // For new cards, do basic validation (not expired, reasonable card number)
            var now = DateTime.Now;
            if (expiryDateTime <= now)
                throw new BusinessRuleViolationException("Card expiry date must be in the future");

            // Basic card number validation (should be 13-19 digits)
            var cardNumberStr = cardNumber.ToString();
            if (cardNumberStr.Length < 13 || cardNumberStr.Length > 19)
                throw new BusinessRuleViolationException("Invalid card number format");
        }

        // Validate stock availability before checkout (batch query)
        var cartISBNs = cart.CartItems.Select(item => item.ISBN).ToList();
        var books = await _bookRepository.GetBookDetailsAsync(cartISBNs);

        // Collect all insufficiencies rather than throwing on first to provide a batch response
        var insuffList = new List<InsufficientStockException.InsufficientItem>();
        foreach (var item in cart.CartItems)
        {
            if (!books.TryGetValue(item.ISBN, out var book))
            {
                throw new NotFoundException("Book", "ISBN", item.ISBN);
            }

            if (book.Quantity < item.Quantity)
            {
                insuffList.Add(new InsufficientStockException.InsufficientItem(item.ISBN, book.Quantity, book.Title));
            }
        }

        if (insuffList.Any())
        {
            throw new InsufficientStockException(insuffList);
        }

        // Calculate total price
        decimal totalPrice = cart.CartItems.Sum(item => item.Quantity * item.UnitPrice);

        // Create order items from cart items
        var orderItems = cart.CartItems.Select(item =>
        {
            var title = books.TryGetValue(item.ISBN, out var b) ? b.Title : item.ISBN;
            return new CustomerOrderItem { ISBN = item.ISBN, OrderNum = 0, Quantity = item.Quantity, UnitPrice = item.UnitPrice, Title = title };
        }).ToList();

        // Create customer order with items and shipping address snapshot
        var orderId = await _orderRepository.AddAsync(
            new CustomerOrder
            {
                OrderNumber = 0,
                TotalPrice = totalPrice,
                Status = OrderStatus.Confirmed,
                OrderDate = DateOnly.FromDateTime(DateTime.Now),
                Username = username,
                ShippingAddress = shippingAddress
            },
            orderItems
        );

        // Update book quantities (deduct from stock) - batch operation
        var quantityChanges = cart.CartItems.ToDictionary(
            item => item.ISBN,
            item => -item.Quantity
        );
        await _bookRepository.UpdateBookQuantitiesAsync(quantityChanges);

        // Clear cart after successful checkout
        await _shoppingCartRepository.ClearCartAsync(cart.CartId);

        return orderId;
    }

    public async Task CreateCartForUserAsync(string username)
    {
        // Check if user already has a cart
        var existingCart = await _shoppingCartRepository.GetCartByUsernameAsync(username);
        if (existingCart == null)
        {
            // Create new cart for user
            await _shoppingCartRepository.CreateCartAsync(username);
        }
    }

    public async Task<int> GetCartItemCountAsync(string username)
    {
        return await _shoppingCartRepository.GetCartItemCountAsync(username);
    }
}