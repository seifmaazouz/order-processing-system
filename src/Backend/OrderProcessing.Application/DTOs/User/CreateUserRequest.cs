using OrderProcessing.Domain.ValueObjects;
namespace OrderProcessing.Application.DTOs.User;

public record CreateUserRequest(
    string Username,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email,
    string Password,
    UserTypes Role
);
