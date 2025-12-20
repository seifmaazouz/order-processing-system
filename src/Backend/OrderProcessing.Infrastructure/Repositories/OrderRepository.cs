using Dapper;
using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.Interfaces;
using OrderProcessing.Domain.Interfaces.Repositories;

namespace OrderProcessing.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public OrderRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Order?> GetByNumberAsync(int orderNumber)
        {
            const string sql = """
                SELECT
                    order_number,
                    total_price,
                    status,
                    order_date,
                    user_id
                FROM orders
                WHERE order_number = @OrderNumber
            """;

            using var connection =await _connectionFactory.CreateConnectionAsync();

            var row = await connection.QuerySingleOrDefaultAsync<dynamic>(
                sql,
                new { OrderNumber = orderNumber }
            );

            if (row is null)
                return null;

            return new Order(
                row.order_number,
                (float)row.total_price,
                row.status,
                DateOnly.FromDateTime(row.order_date),
                row.user_id
            );
        }

        public async Task AddAsync(Order order)
        {
            const string sql = """
                INSERT INTO orders (
                    order_number,
                    total_price,
                    status,
                    order_date,
                    user_id
                )
                VALUES (
                    @OrderNumber,
                    @TotalPrice,
                    @Status,
                    @OrderDate,
                    @Username
                )
            """;

            using var connection =await  _connectionFactory.CreateConnectionAsync();

            await connection.ExecuteAsync(sql, new
            {
                order.OrderNumber,
                order.TotalPrice,
                order.Status,
                OrderDate = order.OrderDate.ToDateTime(TimeOnly.MinValue),
                order.Username
            });
        }

        public async Task UpdateAsync(Order order)
        {
            const string sql = """
                UPDATE orders
                SET
                    total_price = @TotalPrice,
                    status = @Status
                WHERE order_number = @OrderNumber
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            await connection.ExecuteAsync(sql, new
            {
                order.OrderNumber,
                order.TotalPrice,
                order.Status
            });
        }

        public async Task DeleteAsync(int orderNumber)
        {
            const string sql = """
                DELETE FROM orders
                WHERE order_number = @OrderNumber
            """;

            using var connection =await  _connectionFactory.CreateConnectionAsync();

            await connection.ExecuteAsync(sql, new { OrderNumber = orderNumber });
        }
    }
}
