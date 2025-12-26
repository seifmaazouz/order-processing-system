namespace OrderProcessing.Domain.Models;

public record SalesReportReadModel
(
    string Period,
    decimal TotalSalesAmount,
    long TotalTransactionCount
);

public record TopCustomerReadModel
(
    string CustomerName,
    decimal TotalSpent
);

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
