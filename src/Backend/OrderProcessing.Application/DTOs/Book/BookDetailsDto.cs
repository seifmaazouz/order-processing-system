namespace OrderProcessing.Application.DTOs.Book
{
    public record BookDetailsDto
    (
        string ISBN,
        string Title,
        int Year,
        decimal Price,
        int Stock,
        string Category,
        string Publisher,
        List<string> Authors
    );
}