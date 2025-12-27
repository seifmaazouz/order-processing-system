using Dapper;
using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.Interfaces;
using OrderProcessing.Domain.Interfaces.Repositories;
using OrderProcessing.Domain.ValueObjects;

namespace OrderProcessing.Infrastructure.Repositories
{
    public class CustomerOrderRepository : ICustomerOrderRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CustomerOrderRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IReadOnlyList<CustomerOrder>> GetByUsernameAsync(string username)
        {
            const string sql = """
                SELECT
                    OrderID,
                    OrderDate,
                    Status,
                    TotalPrice,
                    Custname
                FROM "Order"
                WHERE Custname = @Username
                ORDER BY OrderDate DESC
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            var rows = await connection.QueryAsync<OrderRow>(sql, new { Username = username });

            return rows.Select(Map).ToList();
        }

        public async Task<CustomerOrder?> GetByOrderNumberAsync(int orderNumber, string username)
        {
            const string sql = """
                SELECT
                    OrderNumber,
                    TotalPrice,
                    Status,
                    OrderDate,
                    Username
                FROM "Order"
                WHERE OrderNumber = @OrderNumber
                  AND Username = @Username
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            var row = await connection.QuerySingleOrDefaultAsync<OrderRow>(
                sql,
                new { OrderID = orderNumber, Username = username }
            );

            return row is null ? null : Map(row);
        }

        public async Task AddAsync(CustomerOrder order)
        {
            const string sql = """
                INSERT INTO "Order" (
                    OrderID,
                    OrderDate,
                    Status,
                    TotalPrice,
                    Custname
                )
                VALUES (
                    @OrderID,
                    @OrderDate,
                    @Status::order_status_enum,
                    @TotalPrice,
                    @Custname
                )
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            await connection.ExecuteAsync(sql, new
            {
                order.OrderNumber,
                order.TotalPrice,
                Status = order.Status.ToString(),
                order.OrderDate,
                order.Username
            });
        }

        public async Task UpdateStatusAsync(int orderNumber, string username)
        {
            const string sql = """
                UPDATE "Order"
                SET Status = @Status::order_status_enum
                WHERE OrderID = @OrderID
                  AND Custname = @Custname
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            await connection.ExecuteAsync(sql, new
            {
                OrderID = orderNumber,
                Custname = username,
                Status = OrderStatus.Cancelled.ToString() // example default
            });
        }

        public async Task DeleteAsync(int orderNumber, string username)
        {
            const string sql = """
                DELETE FROM "Order"
                WHERE OrderID = @OrderID
                  AND Custname = @Custname
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            await connection.ExecuteAsync(sql, new
            {
                OrderID = orderNumber,
                Custname = username
            });
        }

        // ----------------- Helpers -----------------

        private static CustomerOrder Map(OrderRow row)
        {
            Enum.TryParse<OrderStatus>(row.Status, true, out var status);

            return new CustomerOrder(
                row.OrderNumber,
                row.TotalPrice,
                status,
                row.OrderDate,
                row.Username
            );
        }

        private record OrderRow(
            int OrderNumber,
            float TotalPrice,
            string Status,
            DateOnly OrderDate,
            string Username
        );
    }
}
