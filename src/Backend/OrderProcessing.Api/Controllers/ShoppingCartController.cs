using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Application.DTOs.ShoppingCart;
using OrderProcessing.Application.Interfaces;

namespace OrderProcessing.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ShoppingCartController : ControllerBase
{
    private readonly IShoppingCartService _shoppingCartService;

    public ShoppingCartController(IShoppingCartService shoppingCartService)
    {
        _shoppingCartService = shoppingCartService;
    }

    private string GetCurrentUsername()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(ClaimTypes.Name)?.Value
            ?? User.FindFirst("username")?.Value
            ?? throw new UnauthorizedAccessException("Invalid token: username not found");
    }

    [HttpPost("items")]
    public async Task<ActionResult> AddItemToCart([FromBody] AddCartItemDto dto)
    {
        var username = GetCurrentUsername();
        await _shoppingCartService.AddItemToCartAsync(username, dto);
        return Ok();
    }

    [HttpPut("items/{isbn}")]
    public async Task<ActionResult> UpdateItemQuantity(string isbn, [FromBody] UpdateCartItemDto dto)
    {
        var username = GetCurrentUsername();
        await _shoppingCartService.UpdateCartItemAsync(username, dto);
        return Ok();
    }

    [HttpDelete("items/{isbn}")]
    public async Task<ActionResult> RemoveItemFromCart(string isbn)
    {
        var username = GetCurrentUsername();
        await _shoppingCartService.RemoveItemFromCartAsync(username, isbn);
        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<ShoppingCartDetailsDto>> GetCartDetails()
    {
        var username = GetCurrentUsername();
        var cartDetails = await _shoppingCartService.GetCartDetailsAsync(username);
        return Ok(cartDetails);
    }

    [HttpDelete]
    public async Task<ActionResult> ClearCart()
    {
        var username = GetCurrentUsername();
        await _shoppingCartService.ClearCartAsync(username);
        return NoContent();
    }

    [HttpPost("checkout")]
    public async Task<ActionResult<int>> CheckoutCart([FromBody] CheckoutDto dto)
    {
        var username = GetCurrentUsername();
        var orderId = await _shoppingCartService.CheckoutAsync(username, dto);
        return Ok(new { OrderId = orderId });
    }
}