using OrderProcessing.Application.DTOs.Order;
using OrderProcessing.Application.DTOs.Requests;


namespace OrderProcessing.Application.Interfaces
{
    public interface ICustomerOrderService
    {
        Task<IReadOnlyList<CustomerOrderDto>> GetMyOrdersAsync(string token);
        Task<CustomerOrderDto> CreateOrderAsync(string token, CreateOrderRequest request);
    }
}
