namespace OrderProcessing.Application.DTOs.Book
{
    public record UpdateBookDto
    (
        string Title,
        int PublicationYear,
        decimal SellingPrice,
        int Quantity,
        int Threshold,
        int CatID,
        int PubID
    );
}