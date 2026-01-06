using OrderProcessing.Domain.ValueObjects;
namespace OrderProcessing.Application.DTOs.Requests                 ;

public record CreateUserRequest(
    string Username,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email,
    string Password,
    string ShipAddress
);
