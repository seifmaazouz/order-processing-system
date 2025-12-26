using System.Data;
using Dapper;
using OrderProcessing.Domain.Interfaces;
using OrderProcessing.Domain.Models;
using OrderProcessing.Domain.Interfaces.Repositories;

namespace OrderProcessing.Infrastructure.Repositories;

public class ReportRepistory : IReportRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    
    public ReportRepistory(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<SalesReportReadModel> GetTotalSalesPreviousMonthAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var sql = 
        """
            SELECT 
                TO_CHAR(MIN(OrderDate), 'Month YYYY') AS Period,
                COALESCE(SUM(TotalPrice), 0) AS TotalSalesAmount,
                COUNT(*) AS TotalTransactionCount
            FROM "Order"
            WHERE "Status" = 'Confirmed' 
            AND OrderDate >= CURRENT_DATE - INTERVAL '1 month' 
            AND OrderDate <= CURRENT_DATE
        """;

        return await connection.QuerySingleAsync<SalesReportReadModel>(sql);
    }

    public async Task<SalesReportReadModel> GetTotalSalesByDateAsync(DateOnly date)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var sql = 
        """
            SELECT 
                TO_CHAR(@Date, 'YYYY-MM-DD') AS Period,
                COALESCE(SUM(TotalPrice), 0) AS TotalSalesAmount,
                COUNT(*) AS TotalTransactionCount
            FROM "Order"
            WHERE "Status" = 'Confirmed' 
            AND OrderDate = @Date
        """;

        return await connection.QuerySingleAsync<SalesReportReadModel>(sql, new { Date = date.ToDateTime(TimeOnly.MinValue) });
    }

    public async Task<IEnumerable<TopCustomerReadModel>> GetTop5CustomersAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var sql = 
        """
            SELECT CustName AS CustomerName, SUM(TotalPrice) AS TotalSpent
            FROM "Order"
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
            FROM OrderItem AS oi
            JOIN Book AS b ON b.ISBN = oi.ISBN
            JOIN "Order" AS o ON o.OrderID = oi.OrderNum
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
            SELECT oi.ISBN, b.Title, COUNT(oi.OrderNum) as TimesOrderedFromPublisher
            FROM OrderItem AS oi
            JOIN Book AS b ON b.ISBN = oi.ISBN
            JOIN "Order" AS o ON oi.OrderNum = o.OrderID
            JOIN "User" AS u ON o.CustName = u.Username
            WHERE u."Role" = 'Admin' AND oi.ISBN = @Isbn
            GROUP BY oi.ISBN, b.Title
        """;
        return await connection.QuerySingleOrDefaultAsync<BookReplenishmentCountReadModel>(sql, new { Isbn = isbn });
    }
}