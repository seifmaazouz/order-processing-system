using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Domain.Repositories
{
    public interface IOrderRepository{
        Task<Order?>GetByNumberAsync(int OrderNumber);
        Task UpdateAsync(Order OrderNumber);
        Task AddAsync(Order Order);
        Task DeleteAsync(int orderNumber);
        
    }
}