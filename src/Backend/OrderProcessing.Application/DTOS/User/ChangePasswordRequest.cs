namespace OrderProcessing.Application.DTOS.User;

public record ChangePasswordRequest(
    string OldPassword,
    string NewPassword
);
