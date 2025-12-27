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

        var orderDtos = new List<CustomerOrderDto>();

        foreach (var order in orders)
        {
            // Get order items
            var orderItems = await _orderRepository.GetOrderItemsAsync(order.OrderNumber);
            
            // Get book details for each item
            var itemDtos = new List<OrderItemDto>();
            foreach (var item in orderItems)
            {
                var book = await _bookRepository.GetBookDetailsAsync(item.ISBN);
                // If book details not found, use ISBN as title fallback
                itemDtos.Add(new OrderItemDto(
                    item.ISBN,
                    book?.Title ?? item.ISBN,
                    item.Quantity,
                    item.UnitPrice
                ));
            }

            orderDtos.Add(new CustomerOrderDto(
                order.OrderNumber,
                order.TotalPrice,
                order.Status,
                order.OrderDate,
                itemDtos
            ));
        }

        return orderDtos;
    }

    public async Task<CustomerOrderDto> CreateOrderAsync(string token, CreateOrderRequest request)
    {
        var username = _jwtService.GetUsernameFromToken(token);
        if (string.IsNullOrWhiteSpace(username))
            throw new UnauthorizedAccessException("Invalid token.");

        // Calculate total price and create order items
        decimal totalPrice = 0;
        var orderItems = new List<CustomerOrderItem>();
        
        foreach (var item in request.Items)
        {
            var product = await _bookRepository.GetByISBNAsync(item.ISBN);
            if (product is null)
                throw new InvalidOperationException($"Product with ISBN {item.ISBN} not found.");

            var unitPrice = product.SellingPrice;
            totalPrice += unitPrice * item.Quantity;
            orderItems.Add(new CustomerOrderItem(item.ISBN, 0, item.Quantity, unitPrice));
        }

        var newOrder = new CustomerOrder(
            orderNumber: 0, // let DB handle auto-increment
            totalPrice: totalPrice,
            status: OrderStatus.Pending,
            orderDate: DateOnly.FromDateTime(DateTime.UtcNow),
            username: username
        );

        var orderId = await _orderRepository.AddAsync(newOrder, orderItems);

        // Get order items with book details for response
        var orderItemsFromDb = await _orderRepository.GetOrderItemsAsync(orderId);
        var itemDtos = new List<OrderItemDto>();
        foreach (var item in orderItemsFromDb)
        {
            var book = await _bookRepository.GetBookDetailsAsync(item.ISBN);
            // If book details not found, use ISBN as title fallback
            itemDtos.Add(new OrderItemDto(
                item.ISBN,
                book?.Title ?? item.ISBN,
                item.Quantity,
                item.UnitPrice
            ));
        }

        return new CustomerOrderDto(
            orderId,
            newOrder.TotalPrice,
            newOrder.Status,
            newOrder.OrderDate,
            itemDtos
        );
    }
}
