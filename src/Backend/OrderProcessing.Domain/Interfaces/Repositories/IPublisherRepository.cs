using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Domain.Interfaces.Repositories;

public interface IPublisherRepository
{
    Task<IEnumerable<Publisher>> GetAllAsync();
    Task<Publisher?> GetByIdAsync(int id);
}

