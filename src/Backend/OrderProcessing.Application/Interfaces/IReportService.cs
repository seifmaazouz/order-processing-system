using OrderProcessing.Application.DTOs;

namespace OrderProcessing.Application.Interfaces
{
    public interface IReportService
    {
        Task<SalesReportDto> GetTotalSalesPreviousMonthAsync();
        Task<SalesReportDto> GetTotalSalesByDateAsync(DateTime date);
        Task<IEnumerable<TopCustomerDto>> GetTop5CustomersAsync();
        Task<IEnumerable<TopSellingBookDto>> GetTop10SellingBooksAsync();
        Task<BookReplenishmentCountDto?> GetBookReplenishmentCountAsync(string isbn);
    }
}