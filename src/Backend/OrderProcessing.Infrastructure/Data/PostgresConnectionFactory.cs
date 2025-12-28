using System.Data;
using Npgsql;
using OrderProcessing.Domain.Interfaces;

namespace OrderProcessing.Infrastructure.Data
{
    public class PostgresConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public PostgresConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IDbConnection> CreateConnectionAsync()
        {
            try
            {
                var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();
                return connection;
            }
            catch (DllNotFoundException ex) when (ex.Message.Contains("libgssapi_krb5") || ex.Message.Contains("libgssapi"))
            {
                // Handle missing PostgreSQL native libraries
                throw new InvalidOperationException(
                    "PostgreSQL native libraries are missing. " +
                    "Please install PostgreSQL client libraries. " +
                    "On Linux: sudo apt-get install libgssapi-krb5-2 (or equivalent for your distribution). " +
                    "On Windows: Ensure PostgreSQL client is installed. " +
                    $"Original error: {ex.Message}", ex);
            }
            catch (Npgsql.NpgsqlException ex)
            {
                // Provide more helpful error message for connection issues
                throw new InvalidOperationException(
                    $"Failed to connect to PostgreSQL database. " +
                    $"Please ensure PostgreSQL is running and the connection string is correct. " +
                    $"Original error: {ex.Message}", ex);
            }
            catch (Exception ex) when (ex.Message.Contains("libgssapi") || ex.Message.Contains("cannot open shared object file"))
            {
                // Handle missing native library errors
                throw new InvalidOperationException(
                    "PostgreSQL native libraries are missing or cannot be loaded. " +
                    "Please install PostgreSQL client libraries. " +
                    $"Original error: {ex.Message}", ex);
            }
        }
    }
}