using OrderProcessing.Application.DTOs.User;

namespace OrderProcessing.Application.Interfaces;

public interface IUserService
{
    Task<UserDto> CreateAsync(CreateUserRequest request);
    Task<AuthResultDto> LoginAsync(LoginRequest request);
    Task<UserDto> UpdateAsync(UpdateUserRequest request);
    Task DeleteAsync(int userId);

    Task<UserDto> GetByUsernameAsync(string Username);
    Task<IReadOnlyList<UserDto>> GetAllAsync();

    Task ChangePasswordAsync(ChangePasswordRequest request);
}
