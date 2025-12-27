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
                    cardnumber as card_number,
                    expirydate as expiry_date
                FROM creditcard
                WHERE cardnumber = @CardNumber
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            var row = await connection.QuerySingleOrDefaultAsync<dynamic>(
                sql,
                new { CardNumber = cardNumber }
            );

            if (row is null)
                return null;

            // Handle both DateOnly and DateTime from database
            DateOnly expiryDate;
            if (row.expiry_date is DateOnly dateOnly)
            {
                expiryDate = dateOnly;
            }
            else if (row.expiry_date is DateTime dateTime)
            {
                expiryDate = DateOnly.FromDateTime(dateTime.Date);
            }
            else
            {
                // Try to parse as string if needed
                var dateStr = row.expiry_date?.ToString();
                if (string.IsNullOrWhiteSpace(dateStr))
                {
                    throw new InvalidOperationException($"Invalid expiry date format: {row.expiry_date}");
                }
                
                if (!DateOnly.TryParse(dateStr, out DateOnly parsedDate))
                {
                    throw new InvalidOperationException($"Invalid expiry date format: {row.expiry_date}");
                }
                
                expiryDate = parsedDate;
            }

            // Handle both PascalCase and camelCase from Dapper
            var cardNum = row.card_number ?? row.CardNumber;
            if (cardNum == null)
            {
                throw new InvalidOperationException("Card number not found in result");
            }

            return new CreditCard(
                Convert.ToInt64(cardNum),
                expiryDate
            );
        }
        public async Task<IEnumerable<CreditCard>> GetUserCardsAsync(string username)
        {
            const string sql = """
                SELECT
                    cc.cardnumber as card_number,
                    cc.expirydate as expiry_date
                FROM creditcard cc
                INNER JOIN cardholder ch
                    ON ch.cardnumber = cc.cardnumber
                WHERE ch.username = @Username
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            var rows = await connection.QueryAsync<dynamic>(
                sql,
                new { Username = username }
            );

            return rows.Select(row =>
            {
                // Handle both DateOnly and DateTime from database
                DateOnly expiryDate;
                if (row.expiry_date is DateOnly dateOnly)
                {
                    expiryDate = dateOnly;
                }
                else if (row.expiry_date is DateTime dateTime)
                {
                    expiryDate = DateOnly.FromDateTime(dateTime.Date);
                }
                else
                {
                    // Try to parse as string if needed
                    var dateStr = row.expiry_date?.ToString();
                    if (string.IsNullOrWhiteSpace(dateStr))
                    {
                        throw new InvalidOperationException($"Invalid expiry date format: {row.expiry_date}");
                    }
                    
                    if (!DateOnly.TryParse(dateStr, out DateOnly parsedDate))
                    {
                        throw new InvalidOperationException($"Invalid expiry date format: {row.expiry_date}");
                    }
                    
                    expiryDate = parsedDate;
                }

                // Handle both PascalCase and camelCase from Dapper
                var cardNum = row.card_number ?? row.CardNumber;
                if (cardNum == null)
                {
                    throw new InvalidOperationException("Card number not found in result");
                }

                return new CreditCard(
                    Convert.ToInt64(cardNum),
                    expiryDate
                );
            });
        }


        public async Task AddAsync(CreditCard card, string username)
        {
            const string insertCardSql = """
                INSERT INTO creditcard (
                    cardnumber,
                    expirydate
                )
                VALUES (
                    @CardNumber,
                    @ExpiryDate
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
