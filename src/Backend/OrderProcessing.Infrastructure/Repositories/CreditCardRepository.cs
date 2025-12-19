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

        public async Task<CreditCard?> GetByNumberAsync(string cardNumber)
        {
            const string sql = """
                SELECT
                    card_number,
                    expiry_date,
                    username
                FROM credit_cards
                WHERE card_number = @CardNumber
            """;

            using var connection = _connectionFactory.CreateConnection();

            var row = await connection.QuerySingleOrDefaultAsync<dynamic>(
                sql,
                new { CardNumber = cardNumber }
            );

            if (row is null)
                return null;

            return new CreditCard(
                row.card_number,
                DateOnly.FromDateTime(row.expiry_date),
                row.username
            );
        }

        public async Task AddAsync(CreditCard card)
        {
            const string sql = """
                INSERT INTO credit_cards (
                    card_number,
                    expiry_date,
                    username
                )
                VALUES (
                    @CardNumber,
                    @ExpiryDate,
                    @UserName
                )
            """;

            using var connection = _connectionFactory.CreateConnection();

            await connection.ExecuteAsync(sql, new
            {
                card.CardNumber,
                ExpiryDate = card.ExpiryDate.ToDateTime(TimeOnly.MinValue),
                card.UserName
            });
        }

        public async Task UpdateAsync(CreditCard card)
        {
            const string sql = """
                UPDATE credit_cards
                SET
                    expiry_date = @ExpiryDate,
                    username = @UserName
                WHERE card_number = @CardNumber
            """;

            using var connection = _connectionFactory.CreateConnection();

            await connection.ExecuteAsync(sql, new
            {
                card.CardNumber,
                ExpiryDate = card.ExpiryDate.ToDateTime(TimeOnly.MinValue),
                card.UserName
            });
        }

        public async Task DeleteAsync(string cardNumber)
        {
            const string sql = """
                DELETE FROM credit_cards
                WHERE card_number = @CardNumber
            """;

            using var connection = _connectionFactory.CreateConnection();

            await connection.ExecuteAsync(sql, new { CardNumber = cardNumber });
        }
    }
}
