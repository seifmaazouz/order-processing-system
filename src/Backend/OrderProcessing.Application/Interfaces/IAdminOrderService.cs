using OrderProcessing.Application.DTOs.Order;

namespace OrderProcessing.Application.Interfaces
{
    public interface IAdminOrderService
    {
        Task<List<AdminOrderDto>> GetAllOrdersAsync();
        Task<AdminOrderDto?> GetOrderByIdAsync(int orderId);
        Task UpdateOrderStatusAsync(int orderId, string status, string? adminUsername = null);
    }
}
