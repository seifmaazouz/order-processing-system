namespace OrderProcessing.Application.DTOs.CreditCard;

public record AddCreditCardDto(
    string CardholderName,
    long CardNumber,
    string ExpiryDate
);
