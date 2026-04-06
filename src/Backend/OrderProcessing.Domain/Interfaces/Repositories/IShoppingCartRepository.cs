using OrderProcessing.Domain.Models;
using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Domain.Interfaces.Repositories
{
    public interface IShoppingCartRepository
    {
        Task<ShoppingCartReadModel?> GetCartByUsernameAsync(string username);
        Task<int> CreateCartAsync(string username);
        Task<ShoppingCartReadModel> GetOrCreateCartAsync(string username);
        Task AddCartItemAsync(int cartId, CartItem cartItem);
        Task<int> UpdateCartItemAsync(int cartId, CartItemReadModel cartItem);
        Task RemoveCartItemAsync(int cartId, string isbn);
        Task ClearCartAsync(int cartId);
        Task<int> GetCartItemCountAsync(string username);
    }
}