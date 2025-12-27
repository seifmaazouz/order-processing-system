
namespace OrderProcessing.Application.DTOs.ShoppingCart;

public record UpdateCartItemDto
(
	string ISBN,
	int Quantity
);
