using OrderProcessing.Application.DTOs.ShoppingCart;
using OrderProcessing.Domain.Models;
using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Application.Mappings;

public static class ShoppingCartMappingExtensions
{
    public static CartItemDetailsDto ToCartItemDetailsDto(this CartItem item, string title, List<string> authors, int stock)
    {
        return new CartItemDetailsDto(
            item.ISBN,
            title,
            authors,
            item.Quantity,
            item.UnitPrice,
            item.Quantity * item.UnitPrice,
            stock
        );
    }

    // Overload to support read-model mapping used in repository return types
    public static CartItemDetailsDto ToCartItemDetailsDto(this CartItemReadModel item, string title, List<string> authors, int stock)
    {
        return new CartItemDetailsDto(
            item.ISBN,
            title,
            authors,
            item.Quantity,
            item.UnitPrice,
            item.Quantity * item.UnitPrice,
            stock
        );
    }

    public static ShoppingCartDetailsDto ToShoppingCartDetailsDto(this ShoppingCartReadModel cart, List<CartItemDetailsDto> items)
    {
        return new ShoppingCartDetailsDto(
            cart.CartId,
            cart.Username,
            items,
            items.Sum(i => i.TotalPrice)
        );
    }
}
