using System.Data;

namespace OrderProcessing.Domain.Interfaces
{
    public interface IDbConnectionFactory
    {
        Task<IDbConnection> CreateConnectionAsync();
    }
}