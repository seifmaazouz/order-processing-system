namespace OrderProcessing.Application.DTOs.User;

public record AuthResultDto(
    string AccessToken,
    DateTime ExpiresAt,
    string Role
);
