using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Domain.Models;

public record ShoppingCartReadModel(
    int CartId,
    string Username,
    List<CartItem> CartItems
);
