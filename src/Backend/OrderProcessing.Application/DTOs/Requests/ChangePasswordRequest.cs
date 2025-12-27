namespace OrderProcessing.Application.DTOs.Requests;

public record ChangePasswordRequest(
    string Token,
    string OldPassword,
    string NewPassword
);
