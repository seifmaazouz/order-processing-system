using OrderProcessing.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace OrderProcessing.Domain.Interfaces.Repositories;

public interface ICardHolderRepository
{
    Task AddCardHolderAsync(long cardNumber, string username);
    Task RemoveCardHolderAsync(long cardNumber, string username);
    Task<IEnumerable<CardHolder>> GetCardHoldersByCardAsync(long cardNumber);
    Task<IEnumerable<CardHolder>> GetCardsByUserAsync(string username);
    Task<bool> CardHolderExistsAsync(long cardNumber, string username);
}
