using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.Models;

namespace OrderProcessing.Domain.Interfaces.Repositories
{
    public interface IOrderRepository{
        Task<Order?>GetByNumberAsync(int OrderNumber);
        Task UpdateAsync(Order OrderNumber);
        Task AddAsync(Order Order);
        Task DeleteAsync(int orderNumber);
        Task<int> CreateOrderAsync(string username, decimal totalPrice, List<CartItemReadModel> cartItems);
    }
}