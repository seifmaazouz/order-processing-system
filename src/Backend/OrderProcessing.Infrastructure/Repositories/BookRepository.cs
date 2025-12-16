using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.Interfaces;
using OrderProcessing.Domain.Interfaces.Repositories;

namespace OrderProcessing.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public BookRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    // TODO: Implement repository methods
    public Task AddAsync(Book book)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(string isbn)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(string isbn)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Book>> GetBooksBelowStockThresholdAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Book?> GetByISBNAsync(string isbn)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Book book)
    {
        throw new NotImplementedException();
    }
}