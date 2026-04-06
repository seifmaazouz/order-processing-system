namespace OrderProcessing.Api.Models;

public record InsufficientStockItemDto(string isbn, string? title, int available);

public record InsufficientStockResponse(string error, IEnumerable<InsufficientStockItemDto> items);
