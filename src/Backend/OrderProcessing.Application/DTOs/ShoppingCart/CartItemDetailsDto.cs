
namespace OrderProcessing.Application.DTOs.ShoppingCart;

public record CartItemDetailsDto
(
	string ISBN,
	string Title,
	List<string> Authors,
	int Quantity,
	decimal UnitPrice,
	decimal TotalPrice,
	int Stock
);