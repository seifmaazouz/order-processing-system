namespace OrderProcessing.Application.DTOS.User;

public record UserDto(
    string Username,
    string Email,
    string Role
);
