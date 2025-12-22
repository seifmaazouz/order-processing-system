namespace OrderProcessing.Application.DTOS;

public record UpdateUserRequest(
    string Email,
    string Username
);
