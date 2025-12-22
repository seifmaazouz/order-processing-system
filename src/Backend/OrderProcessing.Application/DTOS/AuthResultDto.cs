namespace OrderProcessing.Application.DTOS;

public record AuthResultDto(
    string AccessToken,
    DateTime ExpiresAt
);
