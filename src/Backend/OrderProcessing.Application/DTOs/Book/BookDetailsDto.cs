using OrderProcessing.Domain.ValueObjects;

namespace OrderProcessing.Application.DTOs.Book
{
    public record BookDetailsDto
    {
        public required string ISBN { get; init; }
        public required string Title { get; init; }
        public required int Year { get; init; }
        public required decimal Price { get; init; }
        public required int Stock { get; init; }
        public required CategoryType Category { get; init; }
        public required string Publisher { get; init; }
        public required List<string> Authors { get; init; }
        public bool IsAvailable => Stock > 0;
    }    
}