using OrderProcessing.Application.DTOs.Order;


namespace OrderProcessing.Application.Interfaces
{
    public interface ICustomerOrderService
    {
        Task<IReadOnlyList<CustomerOrderDto>> GetMyOrdersAsync(string token);
    }
}
