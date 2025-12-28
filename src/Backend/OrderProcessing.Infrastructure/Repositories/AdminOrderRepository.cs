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
                    orderid,
                    orderdate,
                    "Status" as status,
                    totalprice,
                    pubid,
                    confirmedby
                FROM adminorder
                ORDER BY orderdate DESC
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            var rows = await connection.QueryAsync<dynamic>(sql);

            var orders = new List<AdminOrder>();
            foreach (var row in rows)
            {
                try
                {
                    // Handle both PascalCase and camelCase from Dapper
                    var orderId = row.orderid ?? row.OrderID ?? row.OrderId;
                    var orderDate = row.orderdate ?? row.OrderDate;
                    var status = row.status ?? row.Status;
                    var totalPrice = row.totalprice ?? row.TotalPrice;
                    var pubId = row.pubid ?? row.PubID ?? row.PublisherId;
                    var confirmedBy = row.confirmedby ?? row.ConfirmedBy ?? row.confirmedBy;

                    // Validate required fields
                    if (orderId == null)
                        throw new InvalidOperationException("Order ID is null");
                    if (orderDate == null)
                        throw new InvalidOperationException("Order date is null");
                    if (status == null)
                        throw new InvalidOperationException("Order status is null");
                    if (totalPrice == null)
                        throw new InvalidOperationException("Total price is null");
                    if (pubId == null)
                        throw new InvalidOperationException("Publisher ID is null");
                    
                    // Convert orderDate to DateOnly
                    DateOnly orderDateOnly = default;
                    if (orderDate is DateOnly dateOnly)
                    {
                        orderDateOnly = dateOnly;
                    }
                    else if (orderDate is DateTime dateTime)
                    {
                        orderDateOnly = DateOnly.FromDateTime(dateTime.Date);
                    }
                    else
                    {
                        var dateStr = orderDate?.ToString();
                        if (string.IsNullOrWhiteSpace(dateStr))
                        {
                            throw new InvalidOperationException($"Invalid order date format: {orderDate}");
                        }
                        DateOnly parsedDate;
                        if (!DateOnly.TryParse(dateStr, out parsedDate))
                        {
                            throw new InvalidOperationException($"Invalid order date format: {orderDate}");
                        }
                        orderDateOnly = parsedDate;
                    }
                    
                    // Parse status - handle both enum string and database string
                    var statusStr = status?.ToString()?.Trim() ?? "";
                    if (string.IsNullOrWhiteSpace(statusStr))
                    {
                        throw new InvalidOperationException("Order status is empty");
                    }
                    
                    // Try to parse status, handling case variations
                    OrderStatus orderStatus;
                    if (!Enum.TryParse<OrderStatus>(statusStr, true, out orderStatus))
                    {
                        // Try common variations
                        var normalizedStatus = statusStr.ToLower();
                        if (normalizedStatus == "pending")
                            orderStatus = OrderStatus.Pending;
                        else if (normalizedStatus == "confirmed")
                            orderStatus = OrderStatus.Confirmed;
                        else if (normalizedStatus == "canceled" || normalizedStatus == "cancelled")
                            orderStatus = OrderStatus.Canceled;
                        else
                            throw new InvalidOperationException($"Invalid order status: {statusStr}. Valid values are: Pending, Confirmed, Canceled");
                    }
                    
                    orders.Add(new AdminOrder(
                        Convert.ToInt32(orderId),
                        orderDateOnly,
                        orderStatus,
                        Convert.ToDecimal(totalPrice),
                        Convert.ToInt32(pubId),
                        confirmedBy?.ToString()
                    ));
                }
                catch (Exception ex)
                {
                    // Log the error with row data for debugging
                    System.Diagnostics.Debug.WriteLine($"Error processing admin order row: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"Row data: orderid={row.orderid}, status={row.status}, orderdate={row.orderdate}");
                    throw new InvalidOperationException($"Error processing admin order: {ex.Message}", ex);
                }
            }

            return orders;
        }

        public async Task<AdminOrder?> GetByOrderIdAsync(int orderId)
        {
            const string sql = """
                SELECT
                    orderid,
                    orderdate,
                    "Status" as status,
                    totalprice,
                    pubid,
                    confirmedby
                FROM adminorder
                WHERE orderid = @OrderId
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            var row = await connection.QuerySingleOrDefaultAsync<dynamic>(
                sql,
                new { OrderId = orderId }
            );

            if (row is null)
                return null;

            // Handle both PascalCase and camelCase from Dapper
            var orderIdValue = row.orderid ?? row.OrderID ?? row.OrderId;
            var orderDate = row.orderdate ?? row.OrderDate;
            var status = row.status ?? row.Status;
            var totalPrice = row.totalprice ?? row.TotalPrice;
            var pubId = row.pubid ?? row.PubID ?? row.PublisherId;
            var confirmedBy = row.confirmedby ?? row.ConfirmedBy ?? row.confirmedBy;
            
            // Convert orderDate to DateOnly
            DateOnly orderDateOnly = default;
            if (orderDate is DateOnly dateOnly)
            {
                orderDateOnly = dateOnly;
            }
            else if (orderDate is DateTime dateTime)
            {
                orderDateOnly = DateOnly.FromDateTime(dateTime.Date);
            }
            else
            {
                var dateStr = orderDate?.ToString();
                if (string.IsNullOrWhiteSpace(dateStr))
                {
                    throw new InvalidOperationException($"Invalid order date format: {orderDate}");
                }
                if (!DateOnly.TryParse(dateStr, out DateOnly parsedDate))
                {
                    throw new InvalidOperationException($"Invalid order date format: {orderDate}");
                }
                orderDateOnly = parsedDate;
            }
            
            // Parse status
            var statusStr = status?.ToString() ?? "";
            if (!Enum.TryParse<OrderStatus>(statusStr, true, out OrderStatus orderStatus))
            {
                throw new InvalidOperationException($"Invalid order status: {statusStr}");
            }
            
            return new AdminOrder(
                Convert.ToInt32(orderIdValue),
                orderDateOnly,
                orderStatus,
                Convert.ToDecimal(totalPrice),
                Convert.ToInt32(pubId),
                confirmedBy?.ToString()
            );
        }

        public async Task<int> AddAsync(AdminOrder order, List<AdminOrderItem> items)
        {
            const string orderSql = """
                INSERT INTO adminorder (orderdate, status, totalprice, pubid, confirmedby)
                VALUES (@OrderDate, @Status, @TotalPrice, @PublisherId, @ConfirmedBy)
                RETURNING orderid
            """;

            const string itemSql = """
                INSERT INTO adminorderitem (isbn, ordernum, quantity, unitprice)
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
                UPDATE adminorder
                SET "Status" = @Status
                WHERE orderid = @OrderId
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
                UPDATE adminorder
                SET "Status" = @Status, confirmedby = @ConfirmedBy
                WHERE orderid = @OrderId
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
                DELETE FROM adminorder
                WHERE orderid = @OrderId
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
                SELECT COUNT(DISTINCT ordernum)
                FROM adminorderitem
                WHERE isbn = @ISBN
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
                    isbn,
                    quantity,
                    unitprice
                FROM adminorderitem
                WHERE ordernum = @OrderId
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            var rows = await connection.QueryAsync<dynamic>(sql, new { OrderId = orderId });

            var items = new List<AdminOrderItem>();
            foreach (var row in rows)
            {
                var isbn = row.isbn ?? row.ISBN;
                var quantity = row.quantity ?? row.Quantity;
                var unitPrice = row.unitprice ?? row.UnitPrice;

                items.Add(new AdminOrderItem(
                    isbn?.ToString() ?? "",
                    orderId,
                    Convert.ToInt32(quantity),
                    Convert.ToDecimal(unitPrice)
                ));
            }

            return items;
        }
    }
}
