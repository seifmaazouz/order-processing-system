namespace OrderProcessing.Application.DTOs.User
{
    public record CreditCardDto
    (
        long CardNumber,
        string ExpiryMonth,
        string ExpiryYear,
        string CardholderName
    );

    
}