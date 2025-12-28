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
        var order = new AdminOrder(1, DateOnly.FromDateTime(DateTime.Now), OrderStatus.Pending, 1000m, 1, "admin1");
        var orderItems = new List<AdminOrderItem>
        {
            new AdminOrderItem(isbn, orderId, 50, 20.00m)
        };

        _mockOrderRepo.Setup(r => r.GetByOrderIdAsync(orderId))
            .ReturnsAsync(order);
        _mockOrderRepo.Setup(r => r.GetOrderItemsAsync(orderId))
            .ReturnsAsync(orderItems);

        // Act
        await _service.UpdateOrderStatusAsync(orderId, "Confirmed");

        // Assert
        _mockBookRepo.Verify(r => r.UpdateBookQuantityAsync(isbn, 50), Times.Once);
        _mockOrderRepo.Verify(r => r.UpdateStatusAsync(orderId, "Confirmed"), Times.Once);
    }

    [Fact]
    public async Task UpdateOrderStatus_WhenNotConfirmed_DoesNotAddStock()
    {
        // Arrange
        var orderId = 1;
        var order = new AdminOrder(1, DateOnly.FromDateTime(DateTime.Now), OrderStatus.Pending, 1000m, 1, "admin1");

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
        var order = new AdminOrder(1, DateOnly.FromDateTime(DateTime.Now), OrderStatus.Confirmed, 1000m, 1, "admin1");

        _mockOrderRepo.Setup(r => r.GetByOrderIdAsync(orderId))
            .ReturnsAsync(order);

        // Act
        await _service.UpdateOrderStatusAsync(orderId, "Confirmed");

        // Assert
        _mockBookRepo.Verify(r => r.UpdateBookQuantityAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        _mockOrderRepo.Verify(r => r.UpdateStatusAsync(orderId, "Confirmed"), Times.Once);
    }

    [Fact]
    public async Task PlacePublisherOrder_WhenItemsEmpty_ThrowsException()
    {
        // Arrange
        var items = new List<AdminOrderItemDto>();

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleViolationException>(
            () => _service.PlacePublisherOrderAsync("admin1", 1, items)
        );
    }

    [Fact]
    public async Task PlacePublisherOrder_WithValidItems_CreatesOrder()
    {
        // Arrange
        var adminUsername = "admin1";
        var publisherId = 1;
        var items = new List<AdminOrderItemDto>
        {
            new AdminOrderItemDto("978-0-123456-01-0", 10, 50.00m)
        };
        var book = new Book(
            "978-0-123456-01-0", "Test Book", 2020, 50.00m, 100, 10, CategoryType.Science, 1
        );

        _mockBookRepo.Setup(r => r.GetByISBNAsync("978-0-123456-01-0"))
            .ReturnsAsync(book);
        _mockOrderRepo.Setup(r => r.AddAsync(It.IsAny<AdminOrder>(), It.IsAny<List<AdminOrderItem>>()))
            .ReturnsAsync(123);

        // Act
        var orderId = await _service.PlacePublisherOrderAsync(adminUsername, publisherId, items);

        // Assert
        orderId.Should().BeGreaterThan(0);
    }
}

