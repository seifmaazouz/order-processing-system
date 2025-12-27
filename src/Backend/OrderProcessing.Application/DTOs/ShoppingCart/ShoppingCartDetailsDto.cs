
namespace OrderProcessing.Application.DTOs.ShoppingCart;

public record ShoppingCartDetailsDto
(
	int CartId,
	string Username,
	List<CartItemDetailsDto> Items,
	decimal TotalPrice
);
