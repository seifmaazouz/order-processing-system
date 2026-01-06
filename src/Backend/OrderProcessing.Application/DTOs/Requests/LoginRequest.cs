namespace OrderProcessing.Application.DTOs.Requests;

public record LoginRequest(
    string Username,
    string Password
);
