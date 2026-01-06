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
                    OrderID AS OrderNumber,
                    OrderDate,
                    "Status",
                    TotalPrice,
                    Custname AS Username,
                    ShippingAddress
                FROM customerorder
                WHERE CustName = @Username
                ORDER BY OrderDate DESC
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            var orders = await connection.QueryAsync<CustomerOrder>(sql, new { Username = username });
            return orders.ToList();
        }

        public async Task<CustomerOrder?> GetByOrderNumberAsync(int orderNumber, string username)
        {
            const string sql = """
                SELECT
                    OrderID AS OrderNumber,
                    TotalPrice,
                    "Status",
                    OrderDate,
                    CustName AS Username,
                    ShippingAddress
                FROM customerorder
                WHERE OrderID = @OrderID
                  AND CustName = @Username
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            var order = await connection.QuerySingleOrDefaultAsync<CustomerOrder>(
                sql,
                new { OrderID = orderNumber, Username = username }
            );

            return order;
        }

        public async Task<int> AddAsync(CustomerOrder order, List<CustomerOrderItem> items)
        {
            const string orderSql = """
                INSERT INTO CustomerOrder (
                    "Status",
                    TotalPrice,
                    CustName,
                    OrderDate,
                    ShippingAddress
                )
                VALUES (
                    @Status::order_status_enum,
                    @TotalPrice,
                    @CustName,
                    @OrderDate,
                    @ShippingAddress
                )
                RETURNING OrderID
            """;

            const string itemSql = """
                INSERT INTO customerorderitem (ISBN, OrderNum, Quantity, UnitPrice)
                VALUES (@ISBN, @OrderNum, @Quantity, @UnitPrice)
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var orderId = await connection.ExecuteScalarAsync<int>(orderSql, new
                {
                    Status = order.Status.ToString(),
                    order.TotalPrice,
                    CustName = order.Username,
                    OrderDate = order.OrderDate.ToDateTime(TimeOnly.MinValue),
                    order.ShippingAddress
                }, transaction);

                if (items != null && items.Count > 0)
                {
                    foreach (var item in items)
                    {
                        await connection.ExecuteAsync(itemSql, new
                        {
                            item.ISBN,
                            OrderNum = orderId,
                            item.Quantity,
                            item.UnitPrice
                        }, transaction);
                    }
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


        public async Task UpdateStatusAsync(int orderNumber, string username)
        {
            const string sql = """
                UPDATE CustomerOrder
                SET "Status" = @Status::order_status_enum
                WHERE OrderID = @OrderID
                  AND CustName = @CustName
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            await connection.ExecuteAsync(sql, new
            {
                OrderID = orderNumber,
                CustName = username,
                Status = OrderStatus.Canceled.ToString()
            });
        }

        public async Task<IReadOnlyList<CustomerOrderItem>> GetOrderItemsAsync(int orderNumber)
        {
            const string sql = """
                SELECT ISBN, OrderNum, Quantity, UnitPrice
                FROM customerorderItem
                WHERE OrderNum = @OrderNum
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            var items = await connection.QueryAsync<CustomerOrderItem>(sql, new { OrderNum = orderNumber });
            return items.ToList();
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


    }
}
