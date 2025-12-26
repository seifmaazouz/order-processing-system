using OrderProcessing.Application.DTOs;
using OrderProcessing.Application.Exceptions;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Application.Mappings;
using OrderProcessing.Domain.Interfaces.Repositories;

namespace OrderProcessing.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IBookRepository _bookRepository;

        public ReportService(IReportRepository reportRepository, IBookRepository bookRepository)
        {
            _reportRepository = reportRepository;
            _bookRepository = bookRepository;
        }

        public async Task<SalesReportDto> GetTotalSalesPreviousMonthAsync()
        {
            var readModel = await _reportRepository.GetTotalSalesPreviousMonthAsync();
            return readModel.ToDto();
        }

        public async Task<SalesReportDto> GetTotalSalesByDateAsync(DateOnly? date)
        {
            if (!date.HasValue)
                throw new BusinessRuleViolationException("Date parameter is required. Format: YYYY-MM-DD (e.g., 2025-01-15)");

            if (date.Value > DateOnly.FromDateTime(DateTime.Today))
                throw new BusinessRuleViolationException("Date cannot be in the future.");

            var readModel = await _reportRepository.GetTotalSalesByDateAsync(date.Value);
            return readModel.ToDto();
        }

        public async Task<IEnumerable<TopCustomerDto>> GetTop5CustomersAsync()
        {
            var readModels = await _reportRepository.GetTop5CustomersAsync();
            return readModels.ToDtoList();
        }

        public async Task<IEnumerable<TopSellingBookDto>> GetTop10SellingBooksAsync()
        {
            var readModels = await _reportRepository.GetTop10SellingBooksAsync();
            return readModels.ToDtoList();
        }

        public async Task<BookReplenishmentCountDto?> GetBookReplenishmentCountAsync(string isbn)
        {
            var exists = await _bookRepository.ExistsAsync(isbn);
            if (!exists) throw new NotFoundException("Book", "ISBN", isbn);

            var readModel = await _reportRepository.GetBookReplenishmentCountAsync(isbn);
            return readModel?.ToDto();
        }
    }
}