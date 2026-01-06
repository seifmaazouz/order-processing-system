using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Api.Models;
using OrderProcessing.Application.DTOs.ShoppingCart;
using OrderProcessing.Application.DTOs.Responses;
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

    [HttpPost("add-item/{isbn}")]
    public async Task<Results<Ok, UnauthorizedHttpResult, NotFound<ErrorResponse>, BadRequest<ErrorResponse>>> AddItemToCart(string isbn)
    {
        var username = GetCurrentUsername();
        await _shoppingCartService.AddItemToCartAsync(username, isbn);
        return TypedResults.Ok();
    }

    [HttpPut("update-item/{isbn}")]
    public async Task<Results<Ok, UnauthorizedHttpResult, NotFound<ErrorResponse>, BadRequest<ErrorResponse>>> UpdateItemQuantity(string isbn, [FromBody] int quantity)
    {
        var username = GetCurrentUsername();
        await _shoppingCartService.UpdateCartItemAsync(username, isbn, quantity);
        return TypedResults.Ok();
    }

    [HttpDelete("remove-item/{isbn}")]
    public async Task<Results<NoContent, UnauthorizedHttpResult, NotFound<ErrorResponse>>> RemoveItemFromCart(string isbn)
    {
        var username = GetCurrentUsername();
        await _shoppingCartService.RemoveItemFromCartAsync(username, isbn);
        return TypedResults.NoContent();
    }

    [HttpGet]
    public async Task<Results<Ok<ShoppingCartDetailsDto>, UnauthorizedHttpResult>> GetCartDetails()
    {
        var username = GetCurrentUsername();
        var cartDetails = await _shoppingCartService.GetCartDetailsAsync(username);
        return TypedResults.Ok(cartDetails);
    }

    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok(new { message = "Test endpoint working", timestamp = DateTime.Now });
    }

    [HttpDelete]
    public async Task<Results<NoContent, UnauthorizedHttpResult>> ClearCart()
    {
        var username = GetCurrentUsername();
        await _shoppingCartService.ClearCartAsync(username);
        return TypedResults.NoContent();
    }

    [HttpPost("checkout")]
    public async Task<Results<Ok<CheckoutResponse>, UnauthorizedHttpResult, BadRequest<ErrorResponse>>> CheckoutCart([FromBody] CheckoutDto dto)
    {
        var username = GetCurrentUsername();
        var orderId = await _shoppingCartService.CheckoutAsync(username, dto);
        return TypedResults.Ok(new CheckoutResponse(orderId));
    }
    
    [HttpGet("count")]
    public async Task<Results<Ok<int>, UnauthorizedHttpResult>> GetCartItemCount()
    {
        var username = GetCurrentUsername();
        var count = await _shoppingCartService.GetCartItemCountAsync(username);
        return TypedResults.Ok(count);
    }
}