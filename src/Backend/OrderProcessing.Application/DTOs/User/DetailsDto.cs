namespace OrderProcessing.Application.DTOs.User
{
    public record DetailsDto(
        string Username,
        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber,
        string Address,
        List<CreditCardDto> CreditCards
    );
}
