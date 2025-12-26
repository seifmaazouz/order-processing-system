namespace OrderProcessing.Application.DTOs;

// For Reports (a) and (b): Total Sales
public record SalesReportDto
(
    string Period,          // "October 2025" or "2025-10-15"
    decimal TotalSalesAmount,
    int TotalTransactionCount
);

// For Report (c): Top 5 Customers
public record TopCustomerDto
(
    string CustomerName,
    string Email,
    decimal TotalSpent
);

// For Report (d): Top 10 Selling Books
public record TopSellingBookDto
(
    string ISBN,
    string Title,
    int TotalCopiesSold
);

// For Report (e): Admin Replenishment Orders Count
public record BookReplenishmentCountDto
(
    string ISBN,
    string Title,
    int TimesOrderedFromPublisher // How many times admin restocked it
);