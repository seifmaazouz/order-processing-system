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
                SELECT
                    OrderID,
                    OrderDate,
                    "Status",
                    TotalPrice,
                    PubID,
                    CustName
                FROM AdminOrder
                ORDER BY OrderDate DESC
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            var rows = await connection.QueryAsync<dynamic>(sql);

            var orders = new List<AdminOrder>();
            foreach (var row in rows)
            {
                orders.Add(new AdminOrder(
                    row.orderid,
                    DateOnly.FromDateTime(row.orderdate),
                    Enum.Parse<OrderStatus>(row.status, true),
                    row.totalprice,
                    row.pubid,
                    row.custname
                ));
            }

            return orders;
        }

        public async Task<AdminOrder?> GetByOrderIdAsync(int orderId)
        {
            const string sql = """
                SELECT
                    OrderID,
                    OrderDate,
                    "Status",
                    TotalPrice,
                    PubID,
                    CustName
                FROM AdminOrder
                WHERE OrderID = @OrderId
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            var row = await connection.QuerySingleOrDefaultAsync<dynamic>(
                sql,
                new { OrderId = orderId }
            );

            if (row is null)
                return null;

            return new AdminOrder(
                row.orderid,
                DateOnly.FromDateTime(row.orderdate),
                Enum.Parse<OrderStatus>(row.status, true),
                row.totalprice,
                row.pubid,
                row.custname
            );
        }

        public async Task<int> AddAsync(AdminOrder order, List<AdminOrderItem> items)
        {
            const string orderSql = """
                INSERT INTO AdminOrder (OrderDate, "Status", TotalPrice, PubID, CustName)
                VALUES (@OrderDate, @Status, @TotalPrice, @PublisherId, @Username)
                RETURNING OrderID
            """;

            const string itemSql = """
                INSERT INTO AdminOrderItem (ISBN, OrderNum, Quantity, UnitPrice)
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
                        order.OrderDate,
                        Status = order.Status.ToString(),
                        order.TotalPrice,
                        order.PublisherId,
                        order.Username
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
                WHERE OrderID = @OrderId
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            await connection.ExecuteAsync(
                sql,
                new { OrderId = orderId, Status = status }
            );
        }

        public async Task DeleteAsync(int orderId)
        {
            const string sql = """
                DELETE FROM AdminOrder
                WHERE OrderID = @OrderId
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
                FROM AdminOrderItem
                WHERE ISBN = @ISBN
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            return await connection.ExecuteScalarAsync<int>(
                sql,
                new { ISBN = isbn }
            );
        }
    }
}
