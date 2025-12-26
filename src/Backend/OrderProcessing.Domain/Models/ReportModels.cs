namespace OrderProcessing.Domain.Models;

public record SalesReportReadModel
(
    string Period,
    decimal TotalSalesAmount,
    int TotalTransactionCount
);

public record TopCustomerReadModel
(
    string CustomerName,
    string Email,
    decimal TotalSpent
);

public record TopSellingBookReadModel
(
    string ISBN,
    string Title,
    int TotalCopiesSold
);

public record BookReplenishmentCountReadModel
(
    string ISBN,
    string Title,
    int TimesOrderedFromPublisher
);
