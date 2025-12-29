using Dapper;
using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.Interfaces;
using OrderProcessing.Domain.Interfaces.Repositories;
using OrderProcessing.Domain.ValueObjects;

namespace OrderProcessing.Infrastructure.Repositories
{
    public class AdminOrderRepository : IAdminOrderRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AdminOrderRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IReadOnlyList<AdminOrder>> GetAllAsync()
        {
            const string sql = """
                SELECT OrderId, OrderDate, "Status", TotalPrice, PublisherId, ConfirmedBy
                FROM adminorder
                ORDER BY OrderDate DESC
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            var orders = (await connection.QueryAsync<AdminOrder>(sql)).ToList();

            return orders;
        }

        public async Task<AdminOrder?> GetByOrderIdAsync(int orderId)
        {
            const string sql = """
                SELECT OrderId, OrderDate, "Status", TotalPrice, PublisherId, ConfirmedBy
                FROM adminorder
                WHERE OrderId = @OrderId
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            var order = await connection.QuerySingleOrDefaultAsync<AdminOrder>(
                sql,
                new { OrderId = orderId }
            );

            return order;
        }

        public async Task<int> AddAsync(AdminOrder order, List<AdminOrderItem> items)
        {
            const string orderSql = """
                INSERT INTO AdminOrder (OrderDate, "Status", TotalPrice, PublisherId, ConfirmedBy)
                VALUES (@OrderDate, @Status, @TotalPrice, @PublisherId, @ConfirmedBy)
                RETURNING OrderId
            """;

            const string itemSql = """
                INSERT INTO adminorderitem (ISBN, OrderNum, Quantity, UnitPrice)
                VALUES (@ISBN, @OrderNum, @Quantity, @UnitPrice)
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var orderId = await connection.QuerySingleAsync<int>(
                    orderSql,
                    new
                    {
                        OrderDate = order.OrderDate.ToDateTime(TimeOnly.MinValue),
                        Status = order.Status.ToString(),
                        order.TotalPrice,
                        order.PublisherId,
                        order.ConfirmedBy
                    },
                    transaction
                );

                foreach (var item in items)
                {
                    await connection.ExecuteAsync(
                        itemSql,
                        new
                        {
                            item.ISBN,
                            OrderNum = orderId,
                            item.Quantity,
                            item.UnitPrice
                        },
                        transaction
                    );
                }

                transaction.Commit();
                return orderId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task UpdateStatusAsync(int orderId, string status)
        {
            const string sql = """
                UPDATE AdminOrder
                SET "Status" = @Status
                WHERE OrderId = @OrderId
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            await connection.ExecuteAsync(
                sql,
                new { OrderId = orderId, Status = status }
            );
        }

        public async Task UpdateStatusAndConfirmedByAsync(int orderId, string status, string confirmedBy)
        {
            const string sql = """
                UPDATE AdminOrder
                SET "Status" = @Status, ConfirmedBy = @ConfirmedBy
                WHERE OrderId = @OrderId
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            await connection.ExecuteAsync(
                sql,
                new { OrderId = orderId, Status = status, ConfirmedBy = confirmedBy }
            );
        }

        public async Task DeleteAsync(int orderId)
        {
            const string sql = """
                DELETE FROM AdminOrder
                WHERE OrderId = @OrderId
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            await connection.ExecuteAsync(
                sql,
                new { OrderId = orderId }
            );
        }

        public async Task<int> GetOrderCountForBookAsync(int isbn)
        {
            const string sql = """
                SELECT COUNT(DISTINCT OrderNum)
                FROM adminorderitem
                WHERE ISBN = @ISBN
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            return await connection.ExecuteScalarAsync<int>(
                sql,
                new { ISBN = isbn }
            );
        }

        public async Task<List<AdminOrderItem>> GetOrderItemsAsync(int orderId)
        {
            const string sql = """
                SELECT
                    ISBN,
                    Quantity,
                    UnitPrice
                FROM adminorderitem
                WHERE OrderNum = @OrderId
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            var items = await connection.QueryAsync<AdminOrderItem>(sql, new { OrderId = orderId });
            return items.ToList();
        }
    }
}
