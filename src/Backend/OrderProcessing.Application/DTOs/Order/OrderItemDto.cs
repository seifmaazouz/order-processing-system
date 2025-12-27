namespace OrderProcessing.Application.DTOs.Order;

public record OrderItemDto(
    int Quantity,
    float UnitPrice
);
