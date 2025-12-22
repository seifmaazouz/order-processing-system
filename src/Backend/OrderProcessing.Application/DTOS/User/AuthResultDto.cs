namespace OrderProcessing.Application.DTOS.User;

public record AuthResultDto(
    string AccessToken,
    DateTime ExpiresAt
);
