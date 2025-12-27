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

        public async Task<int> AddAsync(CustomerOrder order, List<CustomerOrderItem> items)
        {
            const string orderSql = """
                INSERT INTO CustomerOrder (
                    "Status",
                    TotalPrice,
                    CustName,
                    OrderDate
                )
                VALUES (
                    @Status::order_status_enum,
                    @TotalPrice,
                    @CustName,
                    @OrderDate
                )
                RETURNING OrderID
            """;

            const string itemSql = """
                INSERT INTO CustomerOrderItem (ISBN, OrderNum, Quantity, UnitPrice)
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
                    order.OrderDate
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
                FROM CustomerOrderItem
                WHERE OrderNum = @OrderNum
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            var rows = await connection.QueryAsync<OrderItemRow>(sql, new { OrderNum = orderNumber });

            return rows.Select(row => new CustomerOrderItem(
                row.ISBN,
                row.OrderNum,
                row.Quantity,
                row.UnitPrice
            )).ToList();
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
                row.TotalPrice,
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

        private record OrderItemRow(
            string ISBN,
            int OrderNum,
            int Quantity,
            decimal UnitPrice
        );

    }
}
