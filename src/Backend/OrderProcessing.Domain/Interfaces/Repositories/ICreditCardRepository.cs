using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Domain.Interfaces.Repositories
{
    public interface ICreditCardRepository
    {
        Task<CreditCard?> GetByNumberAsync(long CardNumber);
        Task<IEnumerable<CreditCard>> GetUserCardsAsync(string UserName);
        Task AddAsync(CreditCard card, string username);
        Task DeleteAsync(long CardNumber, string Username);
        Task<bool> ValidateCreditCardAsync(long cardNumber, DateTime expiryDate);
    }
}