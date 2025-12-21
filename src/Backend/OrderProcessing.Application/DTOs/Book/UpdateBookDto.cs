using OrderProcessing.Domain.ValueObjects;

namespace OrderProcessing.Application.DTOs.Book
{
    public record UpdateBookDto
    (
        string? Title,
        int? PublicationYear,
        decimal? SellingPrice,
        int? Quantity,
        int? Threshold,
        CategoryType? Category,
        int? PubID,
        List<string>? Authors // Null: no change, Empty: error, Non-empty: set these authors
    );
}