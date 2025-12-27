using OrderProcessing.Application.DTOs.Order;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Application.Security;
using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.Interfaces.Repositories;

namespace OrderProcessing.Application.Services
{
    public class CustomerOrderService : ICustomerOrderService
    {
        private readonly ICustomerOrderRepository _orderRepository;
        private readonly IJwtService _jwtService;

        public CustomerOrderService(
            ICustomerOrderRepository orderRepository,
            IJwtService jwtService)
        {
            _orderRepository = orderRepository;
            _jwtService = jwtService;
        }

        public async Task<IReadOnlyList<CustomerOrderDto>> GetMyOrdersAsync(string token)
        {
            
            var username = _jwtService.GetUsernameFromToken(token);
            if (string.IsNullOrWhiteSpace(username))
                throw new UnauthorizedAccessException("Invalid token.");

            var orders = await _orderRepository.GetByUsernameAsync(username);
            var orderDtos = orders.Select(o => new CustomerOrderDto(
                o.OrderNumber,
                o.TotalPrice,
                o.Status.ToString(),
                o.OrderDate
            )).ToList();

            return orderDtos;
        }
    }
}
