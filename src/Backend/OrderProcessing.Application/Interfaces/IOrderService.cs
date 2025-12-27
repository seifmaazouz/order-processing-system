using OrderProcessing.Application.DTOs.Order;

namespace OrderProcessing.Application.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetOrdersForUserAsync(string username);
    }
}