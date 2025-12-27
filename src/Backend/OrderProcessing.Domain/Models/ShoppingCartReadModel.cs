namespace OrderProcessing.Domain.Models;

public record ShoppingCartReadModel(
    int CartId,
    string Username,
    List<CartItemReadModel> CartItems
);

public record CartItemReadModel(
    string ISBN,
    int Quantity,
    decimal UnitPrice
);
