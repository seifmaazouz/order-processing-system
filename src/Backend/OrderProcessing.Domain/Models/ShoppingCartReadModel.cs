namespace OrderProcessing.Domain.Models;

public record ShoppingCartReadModel(
    int CartId,
    string Username,
    List<CartItemReadModel> CartItems
);
