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


        public async Task<List<AdminOrderDto>> GetAllOrdersAsync()
        {
            var orders = await _adminOrderRepository.GetAllAsync();
            var orderDtos = new List<AdminOrderDto>();

            foreach (var order in orders)
            {
                var items = await _adminOrderRepository.GetOrderItemsAsync(order.OrderId);
                var itemDtos = items.Select(i => new AdminOrderItemDto(i.ISBN, i.Quantity, i.UnitPrice)).ToList();

                orderDtos.Add(new AdminOrderDto(
                    order.OrderId,
                    order.OrderDate,
                    order.Status,
                    order.TotalPrice,
                    order.PublisherId,
                    order.ConfirmedBy,
                    itemDtos
                ));
            }

            return orderDtos;
        }

        public async Task<AdminOrderDto?> GetOrderByIdAsync(int orderId)
        {
            var order = await _adminOrderRepository.GetByOrderIdAsync(orderId);
            if (order == null)
                return null;

            var items = await _adminOrderRepository.GetOrderItemsAsync(orderId);
            var itemDtos = items.Select(i => new AdminOrderItemDto(i.ISBN, i.Quantity, i.UnitPrice)).ToList();

            return new AdminOrderDto(
                order.OrderId,
                order.OrderDate,
                order.Status,
                order.TotalPrice,
                order.PublisherId,
                order.ConfirmedBy,
                itemDtos
            );
        }

        public async Task UpdateOrderStatusAsync(int orderId, string status, string? adminUsername = null)
        {
            var order = await _adminOrderRepository.GetByOrderIdAsync(orderId);
            if (order == null)
                throw new NotFoundException("AdminOrder", "OrderId", orderId.ToString());

            // Parse status string to OrderStatus enum
            if (!Enum.TryParse<OrderStatus>(status, ignoreCase: true, out var orderStatus))
                throw new ArgumentException($"Invalid order status: {status}");

            // If status is being changed to Confirmed, add stock to books and set confirmed by
            if (status.Equals("Confirmed", StringComparison.OrdinalIgnoreCase) && order.Status != OrderStatus.Confirmed)
            {
                if (string.IsNullOrEmpty(adminUsername))
                    throw new ArgumentException("Admin username is required when confirming an order");

                var orderItems = await _adminOrderRepository.GetOrderItemsAsync(orderId);
                foreach (var item in orderItems)
                {
                    await _bookRepository.UpdateBookQuantityAsync(item.ISBN, item.Quantity);
                }

                // Update status and confirmed by
                await _adminOrderRepository.UpdateStatusAndConfirmedByAsync(orderId, orderStatus.ToString(), adminUsername);
            }
            else
            {
                // For other status changes, just update the status
                await _adminOrderRepository.UpdateStatusAsync(orderId, orderStatus.ToString());
            }
        }
    }
}
