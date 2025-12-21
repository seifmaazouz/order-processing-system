using OrderProcessing.Domain.ValueObjects;

namespace OrderProcessing.Application.DTOs.Book
{
    public record CreateBookDto
    (
        string ISBN,
        string Title,
        int PublicationYear,
        decimal SellingPrice,
        int Quantity,
        int Threshold,
        CategoryType Category,
        int PubID,
        List<string> Authors
    );
}