namespace OrderProcessing.Domain.Models;

public record BookSearchFilter
(
    string? Search,
    string? Category,
    string? Author,
    string? Publisher
);