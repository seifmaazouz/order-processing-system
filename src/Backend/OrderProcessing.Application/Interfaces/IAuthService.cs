using OrderProcessing.Application.DTOs.Requests;
using OrderProcessing.Application.DTOs.User;

namespace OrderProcessing.Application.Interfaces;

public interface IAuthService
{
    Task<UserDto> CreateAsync(CreateUserRequest request);
    Task<AuthResultDto> LoginAsync(LoginRequest request);
    Task<UserDto> CreateAdminAsync(CreateUserRequest request);
    Task<string> LogoutAsync(string token);
}
