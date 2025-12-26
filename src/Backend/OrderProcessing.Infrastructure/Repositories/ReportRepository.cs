using OrderProcessing.Domain.Interfaces;
using OrderProcessing.Domain.Models;
using OrderProcessing.Domain.Repositories;

namespace OrderProcessing.Infrastructure.Repositories;

public class ReportRepistory : IReportRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    public ReportRepistory (IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public Task<SalesReportReadModel> GetTotalSalesPreviousMonthAsync()
    {
        throw new NotImplementedException();
    }

    public Task<SalesReportReadModel> GetTotalSalesByDateAsync(DateTime date)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TopSellingBookReadModel>> GetTop10SellingBooksAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TopCustomerReadModel>> GetTop5CustomersAsync()
    {
        throw new NotImplementedException();
    }

    public Task<BookReplenishmentCountReadModel?> GetBookReplenishmentCountAsync(string isbn)
    {
        throw new NotImplementedException();
    }
}