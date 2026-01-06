using OrderProcessing.Domain.ValueObjects;

namespace OrderProcessing.Application.DTOs.Book;

public record SearchBooksQueryDto
{
    public string? Search { get; init; }
    public CategoryType? Category { get; init; }
    public string? Author { get; init; }
    public string? Publisher { get; init; }
}