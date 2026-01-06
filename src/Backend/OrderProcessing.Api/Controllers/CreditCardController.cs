using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Api.Models;
using OrderProcessing.Application.DTOs.CreditCard;
using OrderProcessing.Application.Interfaces;

namespace OrderProcessing.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CreditCardController : ControllerBase
{
    private readonly ICreditCardService _creditCardService;

    public CreditCardController(ICreditCardService creditCardService)
    {
        _creditCardService = creditCardService;
    }

    private string GetCurrentUsername()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(ClaimTypes.Name)?.Value
            ?? User.FindFirst("username")?.Value
            ?? throw new UnauthorizedAccessException("Invalid token: username not found");
    }

    [HttpGet]
    public async Task<Results<Ok<IEnumerable<CreditCardDetailsDto>>, UnauthorizedHttpResult>> GetUserCreditCards()
    {
        var username = GetCurrentUsername();
        var cards = await _creditCardService.GetUserCreditCardsAsync(username);
        return TypedResults.Ok(cards);
    }

    [HttpPost]
    public async Task<Results<Created, UnauthorizedHttpResult, BadRequest<ErrorResponse>>> AddCreditCard([FromBody] AddCreditCardDto dto)
    {
        var username = GetCurrentUsername();
        await _creditCardService.AddCreditCardAsync(username, dto);
        return TypedResults.Created($"/api/creditcard");
    }

    [HttpDelete("{cardNumber}")]
    public async Task<Results<NoContent, BadRequest<ErrorResponse>, ForbidHttpResult, UnauthorizedHttpResult>> DeleteCreditCard(string cardNumber)
    {
        var username = GetCurrentUsername();

        // Verify the card belongs to the authenticated user before deleting
        if (!long.TryParse(cardNumber, out var cardNumLong))
        {
            return TypedResults.BadRequest(new ErrorResponse("Card number must be numeric.", 400));
        }
        var userCards = await _creditCardService.GetUserCreditCardsAsync(username);
        if (!userCards.Any(c => c.CardNumber == cardNumLong))
        {
            return TypedResults.Forbid(); // User doesn't own this card
        }

        await _creditCardService.DeleteCreditCardAsync(cardNumber, username);
        return TypedResults.NoContent();
    }
}
