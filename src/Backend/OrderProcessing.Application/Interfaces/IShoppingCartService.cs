using OrderProcessing.Application.DTOs.ShoppingCart;

namespace OrderProcessing.Application.Interfaces;

public interface IShoppingCartService
{
    Task<ShoppingCartDetailsDto> GetCartDetailsAsync(string username);
    Task AddItemToCartAsync(string username, string isbn);
    Task UpdateCartItemAsync(string username, UpdateCartItemDto updateCartItemDto);
    Task RemoveItemFromCartAsync(string username, string isbn);
    Task ClearCartAsync(string username);
    Task<int> CheckoutAsync(string username, CheckoutDto checkoutDto);
}