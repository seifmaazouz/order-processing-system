namespace OrderProcessing.Application.DTOs.Order;

public record OrderDto(
    int OrderNumber,
    DateOnly OrderDate,
    string Status,
    decimal TotalPrice,
    IReadOnlyList<OrderItemDto> Items
);
