using OrderProcessing.Application.DTOs.ShoppingCart;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Application.Mappings;
using OrderProcessing.Application.Exceptions;
using OrderProcessing.Domain.Interfaces.Repositories;
using OrderProcessing.Domain.Models;
using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.ValueObjects;

namespace OrderProcessing.Application.Services;

public class ShoppingCartService : IShoppingCartService
{
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly IBookRepository _bookRepository;
    private readonly ICreditCardRepository _creditCardRepository;
    private readonly ICustomerOrderRepository _orderRepository;

    public ShoppingCartService(
        IShoppingCartRepository shoppingCartRepository, 
        IBookRepository bookRepository,
        ICreditCardRepository creditCardRepository,
        ICustomerOrderRepository orderRepository)
    {
        _shoppingCartRepository = shoppingCartRepository;
        _bookRepository = bookRepository;
        _creditCardRepository = creditCardRepository;
        _orderRepository = orderRepository;
    }

    public async Task<ShoppingCartDetailsDto> GetCartDetailsAsync(string username)
    {
        var cart = await _shoppingCartRepository.GetCartByUsernameAsync(username);
        
        if (cart == null)
            return new ShoppingCartDetailsDto(0, username, new List<CartItemDetailsDto>(), 0);
        
        // Get all ISBNs at once to avoid N+1 queries
        var isbns = cart.CartItems.Select(i => i.ISBN).ToList();
        var books = new Dictionary<string, (string Title, List<string> Authors)>();
        
        foreach (var isbn in isbns)
        {
            var book = await _bookRepository.GetBookDetailsAsync(isbn);
            if (book != null)
            {
                var authors = (book.AuthorNames ?? "")
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .ToList();
                books[isbn] = (book.Title, authors);
            }
        }
        
        var itemsDto = cart.CartItems.Select(item =>
        {
            var (title, authors) = books.GetValueOrDefault(item.ISBN, (string.Empty, new List<string>()));
            return item.ToCartItemDetailsDto(title, authors);
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
            throw new BusinessRuleViolationException($"Book {book.Title} is currently out of stock");

        var cart = await _shoppingCartRepository.GetCartByUsernameAsync(username);
        
        // Create cart if it doesn't exist
        if (cart == null)
        {
            var newCartId = await _shoppingCartRepository.CreateCartAsync(username);
            cart = new ShoppingCartReadModel(newCartId, username, new List<CartItemReadModel>());
        }

        var cartItem = new CartItemReadModel(
            isbn,
            quantity,
            book.SellingPrice
        );
        await _shoppingCartRepository.AddCartItemAsync(cart.CartId, cartItem);
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

        var cartItem = new CartItemReadModel(
            isbn,
            quantity,
            -1
        );
        var affected = await _shoppingCartRepository.UpdateCartItemAsync(cart.CartId, cartItem);
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
        var cart = await _shoppingCartRepository.GetCartByUsernameAsync(username);
        if (cart == null || cart.CartItems.Count == 0) 
            throw new BusinessRuleViolationException("Cannot checkout an empty cart");

        // Parse expiry date from string (accepts YYYY-MM-DD or ISO format)
        if (string.IsNullOrWhiteSpace(checkoutDto.ExpiryDate))
            throw new BusinessRuleViolationException("Expiry date is required.");

        DateTime expiryDateTime;
        if (!DateTime.TryParse(checkoutDto.ExpiryDate, out expiryDateTime))
        {
            // If direct parsing fails, try to extract date from ISO string format
            if (checkoutDto.ExpiryDate.Contains('T'))
            {
                // Extract just the date part from ISO format (YYYY-MM-DDTHH:mm:ss...)
                var datePart = checkoutDto.ExpiryDate.Split('T')[0];
                if (!DateTime.TryParse(datePart, out expiryDateTime))
                {
                    throw new BusinessRuleViolationException($"Invalid expiry date format: {checkoutDto.ExpiryDate}. Expected YYYY-MM-DD format.");
                }
            }
            else
            {
                throw new BusinessRuleViolationException($"Invalid expiry date format: {checkoutDto.ExpiryDate}. Expected YYYY-MM-DD format.");
            }
        }

        // Validate credit card
        var isValidCard = await _creditCardRepository.ValidateCreditCardAsync(checkoutDto.CardNumber, expiryDateTime);
        if (!isValidCard) throw new BusinessRuleViolationException("Invalid credit card information");

        // Calculate total price
        decimal totalPrice = cart.CartItems.Sum(item => item.Quantity * item.UnitPrice);

        // Create order items from cart items
        var orderItems = cart.CartItems.Select(item => 
            new CustomerOrderItem(item.ISBN, 0, item.Quantity, item.UnitPrice)
        ).ToList();

        // Create customer order with items
        var orderId = await _orderRepository.AddAsync(
            new CustomerOrder(0, totalPrice, OrderStatus.Confirmed, DateOnly.FromDateTime(DateTime.Now), username),
            orderItems
        );

        // Update book quantities (deduct from stock)
        foreach (var item in cart.CartItems)
        {
            await _bookRepository.UpdateBookQuantityAsync(item.ISBN, -item.Quantity);
        }

        // Clear cart after successful checkout
        await _shoppingCartRepository.ClearCartAsync(cart.CartId);

        return orderId;
    }
}