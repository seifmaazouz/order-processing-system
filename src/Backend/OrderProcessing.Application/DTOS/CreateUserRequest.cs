using OrderProcessing.Domain.ValueObjects;
namespace OrderProcessing.Application.DTOS;

public record CreateUserRequest(
    string Username,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email,
    string Password,
    UserTypes Role
);
