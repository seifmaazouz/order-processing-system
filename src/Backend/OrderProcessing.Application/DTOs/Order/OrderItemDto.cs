namespace OrderProcessing.Application.DTOs.Order;

public record OrderItemDto(
    int Quantity,
    decimal UnitPrice
);
