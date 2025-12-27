using OrderProcessing.Application.DTOs.Order;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Application.Exceptions;
using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.Interfaces.Repositories;
using OrderProcessing.Domain.ValueObjects;
using System;

namespace OrderProcessing.Application.Services
{
    public class AdminOrderService : IAdminOrderService
    {
        private readonly IAdminOrderRepository _adminOrderRepository;
        private readonly IBookRepository _bookRepository;

        public AdminOrderService(
            IAdminOrderRepository adminOrderRepository,
            IBookRepository bookRepository)
        {
            _adminOrderRepository = adminOrderRepository;
            _bookRepository = bookRepository;
        }

        public async Task<int> PlacePublisherOrderAsync(string adminUsername, int publisherId, List<AdminOrderItemDto> items)
        {
            if (items == null || items.Count == 0)
                throw new BusinessRuleViolationException("Order must contain at least one item");

            // Calculate total price
            decimal totalPrice = 0;
            var orderItems = new List<AdminOrderItem>();

            foreach (var item in items)
            {
                var book = await _bookRepository.GetByISBNAsync(item.ISBN);
                if (book == null)
                    throw new NotFoundException("Book", "ISBN", item.ISBN);

                totalPrice += item.Quantity * item.UnitPrice;
                orderItems.Add(new AdminOrderItem(item.ISBN, 0, item.Quantity, item.UnitPrice));
            }

            // Create admin order
            var order = new AdminOrder(
                orderId: 0,
                orderDate: DateOnly.FromDateTime(DateTime.UtcNow),
                status: OrderStatus.Pending,
                totalPrice: totalPrice,
                publisherId: publisherId,
                username: adminUsername
            );

            var orderId = await _adminOrderRepository.AddAsync(order, orderItems);

            // Update book quantities (add to stock when order arrives)
            foreach (var item in items)
            {
                await _bookRepository.UpdateBookQuantityAsync(item.ISBN, item.Quantity);
            }

            return orderId;
        }

        public async Task<List<AdminOrderDto>> GetAllOrdersAsync()
        {
            var orders = await _adminOrderRepository.GetAllAsync();
            return orders.Select(o => new AdminOrderDto(
                o.OrderId,
                o.OrderDate,
                o.Status,
                o.TotalPrice,
                o.PublisherId,
                o.Username
            )).ToList();
        }

        public async Task<AdminOrderDto?> GetOrderByIdAsync(int orderId)
        {
            var order = await _adminOrderRepository.GetByOrderIdAsync(orderId);
            if (order == null)
                return null;

            return new AdminOrderDto(
                order.OrderId,
                order.OrderDate,
                order.Status,
                order.TotalPrice,
                order.PublisherId,
                order.Username
            );
        }

        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _adminOrderRepository.GetByOrderIdAsync(orderId);
            if (order == null)
                throw new NotFoundException("AdminOrder", "OrderId", orderId.ToString());

            // Parse status string to OrderStatus enum
            if (!Enum.TryParse<OrderStatus>(status, ignoreCase: true, out var orderStatus))
                throw new ArgumentException($"Invalid order status: {status}");

            await _adminOrderRepository.UpdateStatusAsync(orderId, orderStatus.ToString());
        }
    }
}
