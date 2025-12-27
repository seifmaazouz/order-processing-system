using OrderProcessing.Application.DTOs.Order;
using OrderProcessing.Application.DTOs.Requests;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Application.Security;
using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.Interfaces.Repositories;
using OrderProcessing.Domain.ValueObjects;

public class CustomerOrderService : ICustomerOrderService
{
    private readonly ICustomerOrderRepository _orderRepository;
    private readonly IJwtService _jwtService;
    private readonly IBookRepository _bookRepository; // new dependency

    public CustomerOrderService(
        ICustomerOrderRepository orderRepository,
        IJwtService jwtService,
        IBookRepository bookRepository)
    {
        _orderRepository = orderRepository;
        _jwtService = jwtService;
        _bookRepository = bookRepository;
    }
    public async Task<IReadOnlyList<CustomerOrderDto>> GetMyOrdersAsync(string token)
    {
        var username = _jwtService.GetUsernameFromToken(token);
        
        if (string.IsNullOrWhiteSpace(username))
            throw new UnauthorizedAccessException("Invalid token.");

        var orders = await _orderRepository.GetByUsernameAsync(username);

        var orderDtos = orders
            .Select(o => new CustomerOrderDto(
                o.OrderNumber,
                o.TotalPrice,
                o.Status,
                o.OrderDate
            ))
            .ToList();

        return orderDtos;
    }

    public async Task<CustomerOrderDto> CreateOrderAsync(string token, CreateOrderRequest request)
    {
        var username = _jwtService.GetUsernameFromToken(token);
        if (string.IsNullOrWhiteSpace(username))
            throw new UnauthorizedAccessException("Invalid token.");

        // Calculate total price
        decimal totalPrice = 0;
        foreach (var item in request.Items)
        {
            var product = await _bookRepository.GetByISBNAsync(item.ISBN);
            if (product is null)
                throw new InvalidOperationException($"Product with ISBN {item.ISBN} not found.");

            totalPrice += product.SellingPrice * item.Quantity;
        }

        var newOrder = new CustomerOrder(
            orderNumber: 0, // let DB handle auto-increment
            totalPrice: totalPrice,
            status: OrderStatus.Pending,
            orderDate: DateOnly.FromDateTime(DateTime.UtcNow),
            username: username
        );

        await _orderRepository.AddAsync(newOrder);

        return new CustomerOrderDto(
            newOrder.OrderNumber,
            newOrder.TotalPrice,
            newOrder.Status,
            newOrder.OrderDate
        );
    }
}
