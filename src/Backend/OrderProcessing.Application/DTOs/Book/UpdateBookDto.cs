using OrderProcessing.Domain.ValueObjects;

namespace OrderProcessing.Application.DTOs.Book
{
    public record UpdateBookDto
    {
        public string? Title { get; init; }
        public int? PublicationYear { get; init; }
        public decimal? SellingPrice { get; init; }
        public int? Quantity { get; init; }
        public int? Threshold { get; init; }
        public CategoryType? Category { get; init; }
        public int? PubID { get; init; }
        public List<string>? Authors { get; init; } // Null: no change, Empty: error, Non-empty: set these authors
    }
}