using Dapper;
using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.Models;
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
                    OrderID,
                    OrderDate,
                    "Status",
                    TotalPrice,
                    PubID,
                    CustName
                FROM "Order"
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
                row.OrderID,
                (float)row.TotalPrice,
                row.status,
                DateOnly.FromDateTime(row.order_date),
                row.user_id
            );
        }

        public async Task AddAsync(Order order)
        {
            const string sql = """
                INSERT INTO "Order" (
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
                UPDATE "Order"
                SET
                    TotalPrice = @TotalPrice,
                    "Status" = @Status
                WHERE OrderID = @OrderNumber
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
                DELETE FROM "Order"
                WHERE OrderID = @OrderNumber
            """;

            using var connection =await  _connectionFactory.CreateConnectionAsync();

            await connection.ExecuteAsync(sql, new { OrderNumber = orderNumber });
        }

        public async Task<int> CreateOrderAsync(string username, decimal totalPrice, List<CartItemReadModel> cartItems)
        {
            const string insertOrderSql = """
                INSERT INTO "Order" (OrderDate, "Status", TotalPrice, CustName)
                VALUES (CURRENT_DATE, 'Confirmed', @TotalPrice, @Username)
                RETURNING OrderID
            """;

            const string orderItemSql = """
                INSERT INTO OrderItem (ISBN, OrderNum, Quantity, UnitPrice)
                VALUES (@ISBN, @OrderNum, @Quantity, @UnitPrice)
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var orderId = await connection.QuerySingleAsync<int>(
                    insertOrderSql,
                    new { Username = username, TotalPrice = totalPrice },
                    transaction
                );

                foreach (var item in cartItems)
                {
                    await connection.ExecuteAsync(
                        orderItemSql,
                        new { item.ISBN, OrderNum = orderId, item.Quantity, item.UnitPrice },
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
    }
}
