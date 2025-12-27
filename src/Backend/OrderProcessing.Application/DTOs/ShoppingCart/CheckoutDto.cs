namespace OrderProcessing.Application.DTOs.ShoppingCart;

public record CheckoutDto(
    long CardNumber,
    DateTime ExpiryDate
);
