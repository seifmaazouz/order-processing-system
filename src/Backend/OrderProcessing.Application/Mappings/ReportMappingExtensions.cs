using OrderProcessing.Application.DTOs;
using OrderProcessing.Domain.Models;

namespace OrderProcessing.Application.Mappings;

public static class ReportMappingExtensions
{
    public static SalesReportDto ToDto(this SalesReportReadModel model) =>
        new(model.Period, model.TotalSalesAmount, model.TotalTransactionCount);

    public static TopCustomerDto ToDto(this TopCustomerReadModel model) =>
        new(model.CustomerName ?? "Unknown", model.TotalSpent, model.Email ?? "");

    public static TopSellingBookDto ToDto(this TopSellingBookReadModel model) =>
        new(model.ISBN, model.Title, model.TotalCopiesSold);

    public static BookReplenishmentCountDto ToDto(this BookReplenishmentCountReadModel model) =>
        new(model.ISBN, model.Title, model.TimesOrderedFromPublisher, model.TotalQuantityOrdered);

    public static IEnumerable<TopCustomerDto> ToDtoList(this IEnumerable<TopCustomerReadModel> models) =>
        models.Select(m => m.ToDto());

    public static IEnumerable<TopSellingBookDto> ToDtoList(this IEnumerable<TopSellingBookReadModel> models) =>
        models.Select(m => m.ToDto());
}