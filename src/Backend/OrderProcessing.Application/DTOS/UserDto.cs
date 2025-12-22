namespace OrderProcessing.Application.DTOS;

public record UserDto(
    string Username,
    string Email,
    string Role
);
