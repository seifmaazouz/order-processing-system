using OrderProcessing.Application.DTOS;

namespace OrderProcessing.Application.Interfaces;

public interface IUserService
{
    Task<UserDto> CreateAsync(CreateUserRequest request);
    Task<UserDto> UpdateAsync(UpdateUserRequest request);
    Task DeleteAsync(int userId);

    Task<UserDto> GetByIdAsync(string Username);
    Task<IReadOnlyList<UserDto>> GetAllAsync();

    Task<AuthResultDto> LoginAsync(LoginRequest request);
    Task ChangePasswordAsync(ChangePasswordRequest request);
}
