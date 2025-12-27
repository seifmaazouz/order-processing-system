
namespace OrderProcessing.Application.DTOs.ShoppingCart;

public record AddCartItemDto
(
    string ISBN,
    int Quantity,
    decimal UnitPrice
);