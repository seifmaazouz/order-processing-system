using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Domain.Interfaces.Repositories
{
    public interface ICustomerOrderRepository
    {
        Task<IReadOnlyList<CustomerOrder>> GetByUsernameAsync(string username);
        Task<CustomerOrder?> GetByOrderNumberAsync(int orderNumber, string username);
        Task<int> AddAsync(CustomerOrder order, List<CustomerOrderItem> items);
        Task UpdateStatusAsync(int orderNumber, string username);
        Task DeleteAsync(int orderNumber, string username);
        Task<IReadOnlyList<CustomerOrderItem>> GetOrderItemsAsync(int orderNumber);
    }
}
