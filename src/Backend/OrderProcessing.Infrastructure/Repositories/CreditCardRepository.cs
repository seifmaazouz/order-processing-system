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
                    expiry_date
                FROM credit_cards
                WHERE card_number = @CardNumber
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
                    cc.card_number,
                    cc.expiry_date
                FROM credit_cards cc
                INNER JOIN card_holders ch
                    ON ch.card_number = cc.card_number
                WHERE ch.username = @Username
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
                INSERT INTO credit_cards (
                    card_number,
                    expiry_date
                )
                VALUES (
                    @CardNumber,
                    @ExpiryDate
                )
            """;

            const string insertHolderSql = """
                INSERT INTO card_holders (
                    card_number,
                    username
                )
                VALUES (
                    @CardNumber,
                    @Username
                )
            """;

            using var connection =await  _connectionFactory.CreateConnectionAsync();
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
            catch
            {
                transaction.Rollback();
                throw;
            }
        }


        public async Task DeleteAsync(string cardNumber)
        {
            const string deleteHolderSql = """
                DELETE FROM card_holders
                WHERE card_number = @CardNumber
            """;

            const string deleteCardSql = """
                DELETE FROM credit_cards
                WHERE card_number = @CardNumber
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                await connection.ExecuteAsync(
                    deleteHolderSql,
                    new { CardNumber = cardNumber },
                    transaction
                );

                await connection.ExecuteAsync(
                    deleteCardSql,
                    new { CardNumber = cardNumber },
                    transaction
                );

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
