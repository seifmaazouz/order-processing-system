namespace OrderProcessing.Domain.Models;

public record BookDetailsReadModel
(
    string ISBN,
    string Title,
    int PublicationYear,
    decimal SellingPrice,
    int Quantity,
    int Threshold,
    string CategoryName,
    string PublisherName,
    string AuthorNames
);