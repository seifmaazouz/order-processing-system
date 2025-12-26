using OrderProcessing.Domain.Models;

namespace OrderProcessing.Domain.Repositories
{
    public interface IReportRepository
    {
        // a) Total sales for books in the previous month
        Task<SalesReportReadModel> GetTotalSalesPreviousMonthAsync();

        // b) The total sales for books on a certain day
        Task<SalesReportReadModel> GetTotalSalesByDateAsync(DateTime date);

        // c) Top 5 Customers (Last 3 Months)
        Task<IEnumerable<TopCustomerReadModel>> GetTop5CustomersAsync();

        // d) Top 10 Selling Books (Last 3 Months)
        Task<IEnumerable<TopSellingBookReadModel>> GetTop10SellingBooksAsync();

        // e) Total Number of Times a Specific Book Has Been Ordered (Replenishment)
        Task<BookReplenishmentCountReadModel?> GetBookReplenishmentCountAsync(string isbn);
    }
}