namespace OrderProcessing.Application.DTOs.Order;

public record OrderItemDto(
    string ISBN,
    string Title,
    int Quantity,
    decimal UnitPrice
);
