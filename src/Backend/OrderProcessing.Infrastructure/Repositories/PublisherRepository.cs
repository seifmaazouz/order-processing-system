using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.Interfaces;
using OrderProcessing.Domain.Interfaces.Repositories;
using Dapper;

namespace OrderProcessing.Infrastructure.Repositories;

public class PublisherRepository : IPublisherRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public PublisherRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Publisher>> GetAllAsync()
    {
        const string sql = "SELECT * FROM publisher ORDER BY pubname";
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QueryAsync<Publisher>(sql);
    }

    public async Task<Publisher?> GetByIdAsync(int id)
    {
        const string sql = "SELECT * FROM publisher WHERE pubid = @Id";
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<Publisher>(sql, new { Id = id });
    }
}
