namespace OrderProcessing.Application.DTOs.ShoppingCart;

public record CheckoutDto
{
    // If using a saved card, provide SavedCardNumber only
    public long? SavedCardNumber { get; init; }

    // If using a new card, provide all below
    public long? NewCardNumber { get; init; }
    public string? NewCardExpiryDate { get; init; }
    public string? CardholderName { get; init; }

    // Shipping address (optional, fallback to user profile)
    public string? ShippingAddress { get; init; }
}
