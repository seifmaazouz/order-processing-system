namespace OrderProcessing.Application.DTOs.CreditCard;

public record CreditCardDetailsDto(
    long CardNumber,
    DateTime ExpiryDate
);
