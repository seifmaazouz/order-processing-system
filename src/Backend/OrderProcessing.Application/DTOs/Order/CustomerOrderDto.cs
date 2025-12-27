namespace OrderProcessing.Application.DTOs.Order;

public record CustomerOrderDto(
    int OrderNumber,
        float TotalPrice,
        string Status,
        DateOnly OrderDate
);
