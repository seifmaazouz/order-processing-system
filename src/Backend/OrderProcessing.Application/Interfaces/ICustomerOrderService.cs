using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Application.Interfaces
{
    public interface ICustomerOrderService
    {
        Task<IReadOnlyList<CustomerOrder>> GetMyOrdersAsync(string token);
    }
}
