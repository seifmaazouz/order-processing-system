namespace OrderProcessing.Application.DTOs.User;

public record UserDto(
    string Username,
    string Email,
    string Role
);
