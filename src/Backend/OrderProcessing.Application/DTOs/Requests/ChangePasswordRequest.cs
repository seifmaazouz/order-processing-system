namespace OrderProcessing.Application.DTOs.Requests;

public record ChangePasswordRequest(
    string OldPassword,
    string NewPassword
);
