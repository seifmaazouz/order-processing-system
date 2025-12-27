using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<ActionResult<IEnumerable<CreditCardDetailsDto>>> GetUserCreditCards()
    {
        var username = GetCurrentUsername();
        var cards = await _creditCardService.GetUserCreditCardsAsync(username);
        return Ok(cards);
    }

    [HttpPost]
    public async Task<ActionResult> AddCreditCard([FromBody] AddCreditCardDto dto)
    {
        var username = GetCurrentUsername();
        await _creditCardService.AddCreditCardAsync(username, dto);
        return CreatedAtAction(
            nameof(GetUserCreditCards),
            null,
            null
        );
    }

    [HttpDelete("{cardNumber}")]
    public async Task<ActionResult> DeleteCreditCard(string cardNumber)
    {
        var username = GetCurrentUsername();

        // Verify the card belongs to the authenticated user before deleting
        if (!long.TryParse(cardNumber, out var cardNumLong))
        {
            return BadRequest("Card number must be numeric.");
        }
        var userCards = await _creditCardService.GetUserCreditCardsAsync(username);
        if (!userCards.Any(c => c.CardNumber == cardNumLong))
        {
            return Forbid(); // User doesn't own this card
        }

        await _creditCardService.DeleteCreditCardAsync(cardNumber, username);
        return NoContent();
    }
}
