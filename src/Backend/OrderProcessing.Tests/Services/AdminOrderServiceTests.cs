using Xunit;
using FluentAssertions;
using Moq;
using OrderProcessing.Application.Services;
using OrderProcessing.Application.DTOs.Order;
using OrderProcessing.Application.Exceptions;
using OrderProcessing.Domain.Interfaces.Repositories;
using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.ValueObjects;
using OrderProcessing.Domain.Models;

namespace OrderProcessing.Tests.Services;

public class AdminOrderServiceTests
{
    private readonly Mock<IAdminOrderRepository> _mockOrderRepo;
    private readonly Mock<IBookRepository> _mockBookRepo;
    private readonly AdminOrderService _service;

    public AdminOrderServiceTests()
    {
        _mockOrderRepo = new Mock<IAdminOrderRepository>();
        _mockBookRepo = new Mock<IBookRepository>();
        _service = new AdminOrderService(_mockOrderRepo.Object, _mockBookRepo.Object);
    }

    [Fact]
    public async Task UpdateOrderStatus_WhenConfirmed_AddsStockToBooks()
    {
        // Arrange
        var orderId = 1;
        var isbn = "978-0-123456-01-0";
        var order = new AdminOrder(1, DateTime.Now, "Pending", 1000m, 1, "admin1");
        var orderItems = new List<AdminOrderItem>
        {
            new AdminOrderItem { ISBN = isbn, OrderNum = orderId, Quantity = 50, UnitPrice = 20.00m }
        };

        _mockOrderRepo.Setup(r => r.GetByOrderIdAsync(orderId))
            .ReturnsAsync(order);
        _mockOrderRepo.Setup(r => r.GetOrderItemsAsync(orderId))
            .ReturnsAsync(orderItems);

        // Act
        await _service.UpdateOrderStatusAsync(orderId, "Confirmed", "admin1");

        // Assert
        _mockBookRepo.Verify(r => r.UpdateBookQuantityAsync(isbn, 50), Times.Once);
        _mockOrderRepo.Verify(r => r.UpdateStatusAndConfirmedByAsync(orderId, "Confirmed", "admin1"), Times.Once);
    }

    [Fact]
    public async Task UpdateOrderStatus_WhenNotConfirmed_DoesNotAddStock()
    {
        // Arrange
        var orderId = 1;
        var order = new AdminOrder(1, DateTime.Now, "Pending", 1000m, 1, "admin1");

        _mockOrderRepo.Setup(r => r.GetByOrderIdAsync(orderId))
            .ReturnsAsync(order);

        // Act
        await _service.UpdateOrderStatusAsync(orderId, "Canceled");

        // Assert
        _mockBookRepo.Verify(r => r.UpdateBookQuantityAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        _mockOrderRepo.Verify(r => r.UpdateStatusAsync(orderId, "Canceled"), Times.Once);
    }

    [Fact]
    public async Task UpdateOrderStatus_WhenAlreadyConfirmed_DoesNotAddStockAgain()
    {
        // Arrange
        var orderId = 1;
        var order = new AdminOrder(1, DateTime.Now, "Confirmed", 1000m, 1, "admin1");

        _mockOrderRepo.Setup(r => r.GetByOrderIdAsync(orderId))
            .ReturnsAsync(order);

        // Act
        await _service.UpdateOrderStatusAsync(orderId, "Confirmed");

        // Assert
        _mockBookRepo.Verify(r => r.UpdateBookQuantityAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        _mockOrderRepo.Verify(r => r.UpdateStatusAsync(orderId, "Confirmed"), Times.Once);
    }


}

