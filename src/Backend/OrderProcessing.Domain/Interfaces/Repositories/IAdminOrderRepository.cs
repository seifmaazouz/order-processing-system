using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Domain.Interfaces.Repositories
{
    public interface IAdminOrderRepository
    {
        Task<IReadOnlyList<AdminOrder>> GetAllAsync();
        Task<AdminOrder?> GetByOrderIdAsync(int orderId);
        Task<int> AddAsync(AdminOrder order, List<AdminOrderItem> items);
        Task UpdateStatusAsync(int orderId, string status);
        Task UpdateStatusAndConfirmedByAsync(int orderId, string status, string confirmedBy);
        Task DeleteAsync(int orderId);
        Task<int> GetOrderCountForBookAsync(int isbn);
        Task<List<AdminOrderItem>> GetOrderItemsAsync(int orderId);
    }
}
