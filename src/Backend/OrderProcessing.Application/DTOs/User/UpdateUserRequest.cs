namespace OrderProcessing.Application.DTOs.User;

public record UpdateUserRequest(
    string Email,
    string Username
);
