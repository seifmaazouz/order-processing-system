using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Domain.Interfaces.Repositories
{
    public interface ICustomerOrderRepository
    {
        Task<IReadOnlyList<CustomerOrder>> GetByUsernameAsync(string username);
        Task<CustomerOrder?> GetByOrderNumberAsync(int orderNumber, string username);
        Task<int> AddAsync(CustomerOrder order);
        Task UpdateStatusAsync(int orderNumber, string username);
        Task DeleteAsync(int orderNumber, string username);
    }
}
