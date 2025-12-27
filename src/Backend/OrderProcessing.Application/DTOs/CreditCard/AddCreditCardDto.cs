namespace OrderProcessing.Application.DTOs.CreditCard;

public record AddCreditCardDto(
    long CardNumber,
    DateTime ExpiryDate
);
