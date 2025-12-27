using OrderProcessing.Application.DTOs.Order;

namespace OrderProcessing.Application.Interfaces
{
    public interface IAdminOrderService
    {
        Task<int> PlacePublisherOrderAsync(string adminUsername, int publisherId, List<AdminOrderItemDto> items);
        Task<List<AdminOrderDto>> GetAllOrdersAsync();
        Task<AdminOrderDto?> GetOrderByIdAsync(int orderId);
        Task UpdateOrderStatusAsync(int orderId, string status);
    }
}
