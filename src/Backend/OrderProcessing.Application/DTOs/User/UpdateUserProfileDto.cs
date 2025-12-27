namespace OrderProcessing.Application.DTOs.User;

public record UpdateUserProfileDto(
    string? FirstName,
    string? LastName,
    string? Email,
    string? PhoneNumber,
    string? Address
);
