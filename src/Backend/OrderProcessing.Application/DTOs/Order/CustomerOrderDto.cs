using OrderProcessing.Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace OrderProcessing.Application.DTOs.Order;

public record CustomerOrderDto(
    int OrderNumber,
    decimal TotalPrice,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] OrderStatus Status,
    DateOnly OrderDate,
    IReadOnlyList<OrderItemDto> Items
);
