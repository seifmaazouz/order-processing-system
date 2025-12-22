namespace OrderProcessing.Application.DTOS;

public record LoginRequest(
    string Username,
    string Password
);
