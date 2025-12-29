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
                    order.PubID,
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
                order.PubID,
                order.ConfirmedBy,
                itemDtos
            );
        }

        public async Task UpdateOrderStatusAsync(int orderId, string status, string? adminUsername = null)
        {
            var order = await _adminOrderRepository.GetByOrderIdAsync(orderId);
            if (order == null)
                throw new NotFoundException("AdminOrder", "OrderId", orderId.ToString());

            // Use entity for status validation and transitions
            if (status.Equals("Confirmed", StringComparison.OrdinalIgnoreCase) && order.Status != OrderStatus.Confirmed)
            {
                try
                {
                    order.ConfirmBy(adminUsername!);
                }
                catch (ArgumentException ex)
                {
                    throw new BusinessRuleViolationException(ex.Message);
                }
                var orderItems = await _adminOrderRepository.GetOrderItemsAsync(orderId);
                foreach (var item in orderItems)
                {
                    await _bookRepository.UpdateBookQuantityAsync(item.ISBN, item.Quantity);
                }
                await _adminOrderRepository.UpdateStatusAndConfirmedByAsync(orderId, OrderStatus.Confirmed.ToString(), adminUsername!);
            }
            else
            {
                if (!Enum.TryParse<OrderStatus>(status, ignoreCase: true, out var orderStatus))
                    throw new BusinessRuleViolationException($"Invalid order status: {status}");
                try
                {
                    order.ChangeStatus(orderStatus);
                }
                catch (ArgumentException ex)
                {
                    throw new BusinessRuleViolationException(ex.Message);
                }
                await _adminOrderRepository.UpdateStatusAsync(orderId, orderStatus.ToString());
            }
        }
    }
}
