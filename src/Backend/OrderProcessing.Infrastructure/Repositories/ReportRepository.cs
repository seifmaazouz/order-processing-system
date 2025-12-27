using Dapper;
using OrderProcessing.Domain.Interfaces;
using OrderProcessing.Domain.Models;
using OrderProcessing.Domain.Interfaces.Repositories;

namespace OrderProcessing.Infrastructure.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    
    public ReportRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<SalesReportReadModel> GetTotalSalesPreviousMonthAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var salesSql = 
        """
            SELECT OrderID, OrderDate, TotalPrice
            FROM CustomerOrder 
            WHERE "Status" = 'Confirmed' 
              AND OrderDate >= CURRENT_DATE - INTERVAL '1 month' 
              AND OrderDate <= CURRENT_DATE
        """;

        // this select returns the total revenue from all the sales
        var totalPriceSql =
        """
            SELECT SUM(TotalPrice) AS totalPrice
            FROM CustomerOrder
            WHERE "Status" = 'Confirmed' 
              AND OrderDate >= CURRENT_DATE - INTERVAL '1 month' 
              AND OrderDate <= CURRENT_DATE
        """;

        var sales = await connection.QueryAsync(salesSql);
        var totalPrice = await connection.QuerySingleAsync<decimal?>(totalPriceSql) ?? 0;

        return new SalesReportReadModel
        (
            Period: "Previous Month",
            TotalSalesAmount: totalPrice,
            TotalTransactionCount: sales.Count()
        );
    }

    public async Task<SalesReportReadModel> GetTotalSalesByDateAsync(DateOnly date)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        // admin that inserts the date inserts it here
        var salesSql = 
        """
            SELECT OrderID, OrderDate, TotalPrice
            FROM CustomerOrder
            WHERE "Status" = 'Confirmed'
              AND OrderDate = @Date
        """;

        // this select returns the total revenue from all the sales on that day
        var totalPriceSql =
        """
            SELECT SUM(TotalPrice) AS totalPrice
            FROM CustomerOrder
            WHERE "Status" = 'Confirmed'
              AND OrderDate = @Date
        """;

        var sales = await connection.QueryAsync(salesSql, new { Date = date.ToDateTime(TimeOnly.MinValue) });
        var totalPrice = await connection.QuerySingleAsync<decimal?>(totalPriceSql, new { Date = date.ToDateTime(TimeOnly.MinValue) }) ?? 0;

        return new SalesReportReadModel
        (
            Period: date.ToString("yyyy-MM-dd"),
            TotalSalesAmount: totalPrice,
            TotalTransactionCount: sales.Count()
        );
    }

    public async Task<IEnumerable<TopCustomerReadModel>> GetTop5CustomersAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var sql = 
        """
                SELECT CustName AS CustomerName, SUM(TotalPrice) AS TotalSpent
                FROM CustomerOrder
                WHERE "Status" = 'Confirmed' 
                    AND OrderDate >= CURRENT_DATE - INTERVAL '3 months' 
                    AND OrderDate <= CURRENT_DATE
                GROUP BY CustName
                ORDER BY TotalSpent DESC
                LIMIT 5
        """;
        return await connection.QueryAsync<TopCustomerReadModel>(sql);
    }

    public async Task<IEnumerable<TopSellingBookReadModel>> GetTop10SellingBooksAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var sql =
        """
            SELECT b.ISBN, b.Title, SUM(oi.Quantity) AS TotalCopiesSold
            FROM CustomerOrderItem AS oi
            JOIN Book AS b ON b.ISBN = oi.ISBN
            JOIN CustomerOrder AS o ON o.OrderID = oi.OrderNum
            WHERE o."Status" = 'Confirmed' 
                AND o.OrderDate >= CURRENT_DATE - INTERVAL '3 months' 
                AND o.OrderDate <= CURRENT_DATE
            GROUP BY b.ISBN, b.Title
            ORDER BY TotalCopiesSold DESC
            LIMIT 10
        """;
        return await connection.QueryAsync<TopSellingBookReadModel>(sql);
    }

    public async Task<BookReplenishmentCountReadModel?> GetBookReplenishmentCountAsync(string isbn)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var sql =
        """
            SELECT SUM(Quantity) AS TimesOrderedFromPublisher
            FROM AdminOrderItem
            WHERE ISBN = @Isbn
        """;
        return await connection.QuerySingleOrDefaultAsync<BookReplenishmentCountReadModel>(sql, new { Isbn = isbn });
    }
}