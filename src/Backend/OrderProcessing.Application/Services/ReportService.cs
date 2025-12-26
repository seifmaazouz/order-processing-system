using OrderProcessing.Application.DTOs;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Domain.Repositories;

namespace OrderProcessing.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;

        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<SalesReportDto> GetTotalSalesPreviousMonthAsync()
        {
            var readModel = await _reportRepository.GetTotalSalesPreviousMonthAsync();
            return new
            (
                readModel.Period,
                readModel.TotalSalesAmount,
                readModel.TotalTransactionCount
            );
        }

        public async Task<SalesReportDto> GetTotalSalesByDateAsync(DateTime date)
        {
            var readModel = await _reportRepository.GetTotalSalesByDateAsync(date);
            return new
            (
                readModel.Period,
                readModel.TotalSalesAmount,
                readModel.TotalTransactionCount
            );
        }

        public async Task<IEnumerable<TopCustomerDto>> GetTop5CustomersAsync()
        {
            var readModels = await _reportRepository.GetTop5CustomersAsync();
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TopSellingBookDto>> GetTop10SellingBooksAsync()
        {
            var readModels = await _reportRepository.GetTop10SellingBooksAsync();
            throw new NotImplementedException();
        }

        public async Task<BookReplenishmentCountDto?> GetBookReplenishmentCountAsync(string isbn)
        {
            var readModel = await _reportRepository.GetBookReplenishmentCountAsync(isbn);
            if (readModel == null) return null;
            return new 
            (
                readModel.ISBN,
                readModel.Title,
                readModel.TimesOrderedFromPublisher
            );
        }
    }
}