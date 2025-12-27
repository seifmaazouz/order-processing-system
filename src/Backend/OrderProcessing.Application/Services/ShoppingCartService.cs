using OrderProcessing.Application.DTOs.ShoppingCart;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Application.Mappings;
using OrderProcessing.Application.Exceptions;
using OrderProcessing.Domain.Interfaces.Repositories;
using OrderProcessing.Domain.Models;

namespace OrderProcessing.Application.Services;

public class ShoppingCartService : IShoppingCartService
{
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly IBookRepository _bookRepository;
    private readonly ICreditCardRepository _creditCardRepository;
    private readonly IOrderRepository _orderRepository;

    public ShoppingCartService(
        IShoppingCartRepository shoppingCartRepository, 
        IBookRepository bookRepository,
        ICreditCardRepository creditCardRepository,
        IOrderRepository orderRepository)
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
        
        var itemsDto = new List<CartItemDetailsDto>();
        foreach (var item in cart.CartItems)
        {
            var book = await _bookRepository.GetBookDetailsAsync(item.ISBN);
            var title = book?.Title ?? string.Empty;
            var authors = (book?.AuthorNames ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();
            itemsDto.Add(item.ToCartItemDetailsDto(title, authors));
        }
        return cart.ToShoppingCartDetailsDto(itemsDto);
    }

    public async Task AddItemToCartAsync(string username, AddCartItemDto addCartItemDto)
    {
        var cart = await _shoppingCartRepository.GetCartByUsernameAsync(username);
        
        // Create cart if it doesn't exist
        if (cart == null)
        {
            var newCartId = await _shoppingCartRepository.CreateCartAsync(username);
            cart = new ShoppingCartReadModel(newCartId, username, new List<CartItemReadModel>());
        }

        var cartItem = new CartItemReadModel(
            addCartItemDto.ISBN,
            addCartItemDto.Quantity,
            addCartItemDto.UnitPrice
        );
        await _shoppingCartRepository.AddCartItemAsync(cart.CartId, cartItem);
    }

    public async Task UpdateCartItemAsync(string username, UpdateCartItemDto updateCartItemDto)
    {
        var cart = await _shoppingCartRepository.GetCartByUsernameAsync(username);
        if (cart == null)
        {
            throw new NotFoundException("Shopping cart", "username", username);
        }

        var cartItem = new CartItemReadModel(
            updateCartItemDto.ISBN,
            updateCartItemDto.Quantity,
            -1
        );
        var affected = await _shoppingCartRepository.UpdateCartItemAsync(cart.CartId, cartItem);
        if (affected == 0)
        {
            throw new NotFoundException("Cart item", "ISBN", updateCartItemDto.ISBN);
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

        // Validate credit card
        var isValidCard = await _creditCardRepository.ValidateCreditCardAsync(checkoutDto.CardNumber, checkoutDto.ExpiryDate);
        if (!isValidCard) throw new BusinessRuleViolationException("Invalid credit card information");

        // Calculate total price
        decimal totalPrice = 0;
        foreach (var item in cart.CartItems)
        {
            totalPrice += item.Quantity * item.UnitPrice;
        }

        // Create order
        var orderId = await _orderRepository.CreateOrderAsync(username, totalPrice, cart.CartItems);

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