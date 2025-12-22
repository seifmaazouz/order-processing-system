namespace OrderProcessing.Application.DTOS.User;

public record LoginRequest(
    string Username,
    string Password
);
