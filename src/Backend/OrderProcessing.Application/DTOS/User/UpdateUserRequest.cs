namespace OrderProcessing.Application.DTOS.User;

public record UpdateUserRequest(
    string Email,
    string Username
);
