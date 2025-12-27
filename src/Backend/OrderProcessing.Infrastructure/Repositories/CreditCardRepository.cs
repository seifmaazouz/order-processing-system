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
                SELECT
                    CardNumber,
                    ExpiryDate
                FROM CreditCard
                WHERE CardNumber = @CardNumber
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            var row = await connection.QuerySingleOrDefaultAsync<dynamic>(
                sql,
                new { CardNumber = cardNumber }
            );

            if (row is null)
                return null;

            return new CreditCard(
                row.card_number,
                DateOnly.FromDateTime(row.expiry_date)
            );
        }
        public async Task<IEnumerable<CreditCard>> GetUserCardsAsync(string username)
        {
            const string sql = """
                SELECT
                    cc.CardNumber,
                    cc.ExpiryDate
                FROM CreditCard cc
                INNER JOIN CardHolder ch
                    ON ch.CardNumber = cc.CardNumber
                WHERE ch.Username = @Username
            """;

            using var connection =await  _connectionFactory.CreateConnectionAsync();

            var rows = await connection.QueryAsync<dynamic>(
                sql,
                new { Username = username }
            );

            return rows.Select(row =>
                new CreditCard(
                    row.card_number,
                    DateOnly.FromDateTime(row.expiry_date)
                )
            );
        }


        public async Task AddAsync(CreditCard card, string username)
        {
            const string insertCardSql = """
                INSERT INTO CreditCard (
                    CardNumber,
                    ExpiryDate
                )
                VALUES (
                    @CardNumber,
                    @ExpiryDate
                )
            """;

            const string insertHolderSql = """
                INSERT INTO CardHolder (
                    CardNumber,
                    Username
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
                        ExpiryDate = card.ExpiryDate.ToDateTime(TimeOnly.MinValue)
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
            const string countHoldersSql = @"SELECT COUNT(*) FROM CardHolder WHERE CardNumber = @CardNumber";
            const string deleteHolderSql = @"DELETE FROM CardHolder WHERE CardNumber = @CardNumber AND Username = @Username";
            const string deleteCardSql = @"DELETE FROM CreditCard WHERE CardNumber = @CardNumber";

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
            const string sql = """
                SELECT COUNT(1)
                FROM CreditCard
                WHERE CardNumber = @CardNumber
                AND ExpiryDate >= @ExpiryDate
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();
            var count = await connection.QuerySingleAsync<int>(
                sql,
                new { CardNumber = cardNumber, ExpiryDate = expiryDate }
            );

            return count > 0;
        }
    }
}
