using OrderProcessing.Application.DTOs.Order;
using OrderProcessing.Application.DTOs.Requests;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Application.Security;
using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.Interfaces.Repositories;
using OrderProcessing.Domain.ValueObjects;

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
        public async Task<CustomerOrderDto> CreateOrderAsync(string token, CreateOrderRequest request)
        {
            var username = _jwtService.GetUsernameFromToken(token);
            if (string.IsNullOrWhiteSpace(username))
                throw new UnauthorizedAccessException("Invalid token.");

            // You can generate OrderNumber however you like, e.g., auto-increment from DB
            var newOrder = new CustomerOrder(
                orderNumber: 0, // let DB handle auto-increment if configured
                totalPrice: request.TotalPrice,
                status: OrderStatus.Pending,
                orderDate: DateOnly.FromDateTime(DateTime.UtcNow),
                username: username
            );

            await _orderRepository.AddAsync(newOrder);

            return new CustomerOrderDto(
                newOrder.OrderNumber,
                newOrder.TotalPrice,
                newOrder.Status.ToString(),
                newOrder.OrderDate
            );
        }
    }
}
