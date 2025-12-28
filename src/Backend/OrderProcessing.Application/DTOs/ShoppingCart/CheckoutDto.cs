namespace OrderProcessing.Application.DTOs.ShoppingCart;

public record CheckoutDto(
    long CardNumber,
    string ExpiryDate,
    string? ShippingAddress
);
