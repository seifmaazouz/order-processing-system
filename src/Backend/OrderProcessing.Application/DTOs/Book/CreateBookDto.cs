using OrderProcessing.Domain.ValueObjects;

namespace OrderProcessing.Application.DTOs.Book
{
    public record CreateBookDto
    {
        public required string ISBN { get; init; }
        public required string Title { get; init; }
        public required int PublicationYear { get; init; }
        public required decimal SellingPrice { get; init; }
        public required int Quantity { get; init; }
        public required int Threshold { get; init; }
        public required CategoryType Category { get; init; }
        public required int PubID { get; init; }
        public required List<string> Authors { get; init; }
    }
}