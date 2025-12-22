namespace OrderProcessing.Application.DTOS;

public record ChangePasswordRequest(
    string OldPassword,
    string NewPassword
);
