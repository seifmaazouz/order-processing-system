using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Domain.Interfaces.Repositories
{
    public interface ICreditCardRepository
    {
        Task<CreditCard?> GetByNumberAsync(string CardNumber);
        Task<IEnumerable<CreditCard>> GetUserCardsAsync(string UserName);
        Task AddAsync(CreditCard card, string username);
        Task DeleteAsync(string CardNumber);
    }
}