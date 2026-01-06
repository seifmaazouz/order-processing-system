using OrderProcessing.Application.DTOs.CreditCard;

namespace OrderProcessing.Application.Interfaces;

public interface ICreditCardService
{
    Task<CreditCardDetailsDto?> GetCreditCardByNumberAsync(long cardNumber);
    Task<IEnumerable<CreditCardDetailsDto>> GetUserCreditCardsAsync(string username);
    Task AddCreditCardAsync(string username, AddCreditCardDto dto);
    Task DeleteCreditCardAsync(string cardNumber, string username);
}
