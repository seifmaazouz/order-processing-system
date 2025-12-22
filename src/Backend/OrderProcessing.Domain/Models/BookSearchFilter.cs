namespace OrderProcessing.Domain.Models;

public record BookSearchFilter
(
    string? ISBN,
    string? Title,
    string? Category,
    string? Author,
    string? Publisher
);