namespace OrderProcessing.Application.DTOs.User
{
    public record CreditCardDto
    (
        string CardNumber,
        string ExpiryMonth,
        string ExpiryYear
    );

    
}