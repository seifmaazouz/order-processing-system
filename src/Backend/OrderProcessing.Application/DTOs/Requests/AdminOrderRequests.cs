using OrderProcessing.Application.DTOs.Order;

namespace OrderProcessing.Application.DTOs.Requests;

public record PlacePublisherOrderRequest(
    int PublisherId,
    List<AdminOrderItemDto> Items
);

public record UpdateOrderStatusRequest(string Status);
