namespace OrderProcessing.Application.DTOs.User;

public record ChangePasswordRequest(
    string OldPassword,
    string NewPassword
);
