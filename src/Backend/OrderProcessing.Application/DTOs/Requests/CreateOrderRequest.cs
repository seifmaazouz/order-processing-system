using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.ValueObjects;

namespace OrderProcessing.Application.DTOs.Requests
{
    public record CreateOrderRequest(
        List<OrderItemRequest> Items,
        string? ShippingAddress = null
        
    );
}