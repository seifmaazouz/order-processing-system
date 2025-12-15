using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Domain.Repositories
{
    public interface ICreditCardRepository
    {
        Task<CreditCard?> GetByNumberAsync(string CardNumber);
        Task AddAsync(CreditCard Card);
        Task UpdateAsync(CreditCard Card);
        Task DeleteAsync(string CardNumber);
    }
}