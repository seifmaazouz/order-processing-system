using OrderProcessing.Application.DTOs.CreditCard;
using OrderProcessing.Application.Exceptions;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.Interfaces.Repositories;

namespace OrderProcessing.Application.Services;

public class CreditCardService : ICreditCardService
{
    private readonly ICreditCardRepository _creditCardRepository;

    public CreditCardService(ICreditCardRepository creditCardRepository)
    {
        _creditCardRepository = creditCardRepository;
    }

    public async Task<CreditCardDetailsDto?> GetCreditCardByNumberAsync(long cardNumber)
    {
        var card = await _creditCardRepository.GetByNumberAsync(cardNumber);
        return card is not null
            ? new CreditCardDetailsDto(card.CardNumber, card.ExpiryDate.ToDateTime(TimeOnly.MinValue))
            : null;
    }

    public async Task<IEnumerable<CreditCardDetailsDto>> GetUserCreditCardsAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new BusinessRuleViolationException("Username is required");

        var cards = await _creditCardRepository.GetUserCardsAsync(username);
        
        return cards.Select(c => new CreditCardDetailsDto(
            c.CardNumber,
            c.ExpiryDate.ToDateTime(TimeOnly.MinValue)
        ));
    }

    public async Task AddCreditCardAsync(string username, AddCreditCardDto dto)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new BusinessRuleViolationException("Username is required");

        if (dto.ExpiryDate < DateTime.Now)
            throw new BusinessRuleViolationException("Cannot add expired credit card");

        // Check if card already exists for this user
        var existingCards = await _creditCardRepository.GetUserCardsAsync(username);
        if (existingCards.Any(c => c.CardNumber == dto.CardNumber))
            throw new DuplicateResourceException($"Credit card {dto.CardNumber} already exists for user {username}");

        var card = new CreditCard(
            dto.CardNumber,
            DateOnly.FromDateTime(dto.ExpiryDate)
        );

        await _creditCardRepository.AddAsync(card, username);
    }

    public async Task DeleteCreditCardAsync(string cardNumber, string username)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            throw new BusinessRuleViolationException("Card number is required");
        if (string.IsNullOrWhiteSpace(username))
            throw new BusinessRuleViolationException("Username is required");

        if (!long.TryParse(cardNumber, out var cardNumLong))
            throw new BusinessRuleViolationException("Card number must be numeric.");

        var card = await _creditCardRepository.GetByNumberAsync(cardNumLong);
        if (card is null)
            throw new NotFoundException($"Credit card {cardNumber} not found");

        await _creditCardRepository.DeleteAsync(cardNumLong, username);
    }
}
