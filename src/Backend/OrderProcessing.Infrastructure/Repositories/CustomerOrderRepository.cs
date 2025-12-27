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
                    "Status",
                    TotalPrice,
                    Custname
                FROM CustomerOrder
                WHERE CustName = @Username
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
                    OrderID,
                    TotalPrice,
                    "Status",
                    OrderDate,
                    CustName
                FROM CustomerOrder
                WHERE OrderID = @OrderID
                  AND CustName = @Username
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
                INSERT INTO CustomerOrder (
                    OrderID,
                    OrderDate,
                    "Status",
                    TotalPrice,
                    CustName
                )
                VALUES (
                    @OrderID,
                    @OrderDate,
                    @"Status"::order_status_enum,
                    @TotalPrice,
                    @CustName
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
                UPDATE CustomerOrder
                SET "Status" = @"Status"::order_status_enum
                WHERE OrderID = @OrderID
                  AND CustName = @CustName
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
                DELETE FROM CustomerOrder
                WHERE OrderID = @OrderID
                  AND CustName = @CustName
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
                row.OrderID,
                (float)row.TotalPrice,  // convert decimal to float if your domain uses float
                status,
                row.OrderDate,
                row.CustName
            );
        }


        private record OrderRow(
            int OrderID,
            DateOnly OrderDate,
            string Status,
            decimal TotalPrice,
            string CustName
        );

    }
}
