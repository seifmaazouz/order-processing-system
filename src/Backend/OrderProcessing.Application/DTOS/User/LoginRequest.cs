namespace OrderProcessing.Application.DTOs.User;

public record LoginRequest(
    string Username,
    string Password
);
