namespace OrderProcessing.Domain.Models;

public record SalesReportReadModel
(
    string Period,
    decimal TotalSalesAmount,
    long TotalTransactionCount
);

public class TopCustomerReadModel
{
    public string? CustomerName { get; set; }
    public decimal TotalSpent { get; set; }
    public string? Email { get; set; }

    // Parameterless constructor for Dapper
    public TopCustomerReadModel() { }
}

public record TopSellingBookReadModel
(
    string ISBN,
    string Title,
    long TotalCopiesSold
);

public record BookReplenishmentCountReadModel
(
    string ISBN,
    string Title,
    long TimesOrderedFromPublisher
);
