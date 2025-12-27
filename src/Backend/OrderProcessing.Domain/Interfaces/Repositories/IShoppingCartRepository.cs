using OrderProcessing.Domain.Models;

namespace OrderProcessing.Domain.Interfaces.Repositories
{
    public interface IShoppingCartRepository
    {
        Task<ShoppingCartReadModel?> GetCartByUsernameAsync(string username);
        Task<int> CreateCartAsync(string username);
        Task AddCartItemAsync(int cartId, CartItemReadModel cartItem);
        Task<int> UpdateCartItemAsync(int cartId, CartItemReadModel cartItem);
        Task RemoveCartItemAsync(int cartId, string isbn);
        Task ClearCartAsync(int cartId);
    }
}