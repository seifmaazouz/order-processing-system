using Dapper;
using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.Interfaces;
using OrderProcessing.Domain.Interfaces.Repositories;

namespace OrderProcessing.Infrastructure.Repositories
{
    public class CreditCardRepository : ICreditCardRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CreditCardRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<CreditCard?> GetByNumberAsync(long cardNumber)
        {
            const string sql = """
                SELECT CardNumber, ExpiryDate, CardholderName
                FROM creditcard
                WHERE CardNumber = @CardNumber
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            return await connection.QuerySingleOrDefaultAsync<CreditCard>(
                sql,
                new { CardNumber = cardNumber }
            );
        }
        public async Task<IEnumerable<CreditCard>> GetUserCardsAsync(string username)
        {
            const string sql =
            """
                SELECT cc.CardNumber, cc.ExpiryDate, cc.CardholderName
                FROM creditcard cc
                INNER JOIN cardholder ch
                    ON ch.CardNumber = cc.CardNumber
                WHERE ch.Username = @Username
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            return await connection.QueryAsync<CreditCard>(
                sql,
                new { Username = username }
            );
        }


        public async Task AddAsync(CreditCard card, string username)
        {
            const string insertCardSql = """
                INSERT INTO creditcard (
                    cardnumber,
                    expirydate,
                    cardholdername
                )
                VALUES (
                    @CardNumber,
                    @ExpiryDate,
                    @CardholderName
                )
            """;

            const string insertHolderSql = """
                INSERT INTO cardholder (
                    cardnumber,
                    username
                )
                VALUES (
                    @CardNumber,
                    @Username
                )
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                await connection.ExecuteAsync(
                    insertCardSql,
                    new
                    {
                        card.CardNumber,
                        ExpiryDate = card.ExpiryDate.ToDateTime(TimeOnly.MinValue),
                        card.CardholderName
                    },
                    transaction
                );
                await connection.ExecuteAsync(
                    insertHolderSql,
                    new
                    {
                        card.CardNumber,
                        Username = username
                    },
                    transaction
                );
                transaction.Commit();
            }
            catch (Npgsql.PostgresException ex)
            {
                transaction.Rollback();
                throw PostgresExceptionTranslator.Translate(ex);
            }
        }


        public async Task DeleteAsync(long cardNumber, string username)
        {
            const string countHoldersSql = @"SELECT COUNT(*) FROM cardholder WHERE cardnumber = @CardNumber";
            const string deleteHolderSql = @"DELETE FROM cardholder WHERE cardnumber = @CardNumber AND username = @Username";
            const string deleteCardSql = @"DELETE FROM creditcard WHERE cardnumber = @CardNumber";

            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Remove the mapping for this user
                await connection.ExecuteAsync(
                    deleteHolderSql,
                    new { CardNumber = cardNumber, Username = username },
                    transaction
                );

                // Check if any mappings remain
                var remaining = await connection.QuerySingleAsync<int>(
                    countHoldersSql,
                    new { CardNumber = cardNumber },
                    transaction
                );

                if (remaining == 0)
                {
                    await connection.ExecuteAsync(
                        deleteCardSql,
                        new { CardNumber = cardNumber },
                        transaction
                    );
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> ValidateCreditCardAsync(long cardNumber, DateTime expiryDate)
        {
            try
            {
                // Convert DateTime to DateOnly for comparison (extract just the date part)
                var expiryDateOnly = DateOnly.FromDateTime(expiryDate.Date);
                
                const string sql = """
                    SELECT COUNT(1)
                    FROM creditcard
                    WHERE cardnumber = @CardNumber
                    AND expirydate >= @ExpiryDate
                """;

                using var connection = await _connectionFactory.CreateConnectionAsync();
                var count = await connection.QuerySingleAsync<int>(
                    sql,
                    new { CardNumber = cardNumber, ExpiryDate = expiryDateOnly.ToDateTime(TimeOnly.MinValue) }
                );

                return count > 0;
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                System.Diagnostics.Debug.WriteLine($"Error validating credit card {cardNumber}: {ex.Message}");
                throw;
            }
        }
    }
}
