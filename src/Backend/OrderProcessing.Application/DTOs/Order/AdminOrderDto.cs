using OrderProcessing.Domain.ValueObjects;

namespace OrderProcessing.Application.DTOs.Order;

public record AdminOrderDto(
    int OrderId,
    DateOnly OrderDate,
    OrderStatus Status,
    decimal TotalPrice,
    int PublisherId,
    string Username
);

public record AdminOrderItemDto(
    string ISBN,
    int Quantity,
    decimal UnitPrice
);
