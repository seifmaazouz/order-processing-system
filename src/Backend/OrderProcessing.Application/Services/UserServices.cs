using OrderProcessing.Application.DTOS.User;
using OrderProcessing.Domain.Interfaces.Repositories;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Application.Security;
using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Application.Services
{
    public class UserServices : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        public UserServices(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher
        )
    {
        _userRepository = userRepository;
        _passwordHasher=passwordHasher;
    }
        public Task ChangePasswordAsync(ChangePasswordRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDto> CreateAsync(CreateUserRequest request)
        {
            var existedUser= _userRepository.GetByUserNameAsync(request.Username);
            if(existedUser is not null)
            {
                throw new InvalidOperationException("Username already exists");
            }
            string password=_passwordHasher.HashPassword(request.Password);
            var user = new User(
                 request.Username,
                 request.Email,
                 request.PhoneNumber,
                 request.FirstName,
                 request.LastName,
                 password,
                 request.Role
            );
            await _userRepository.AddAsync(user);
            return new UserDto(
                user.Username,
                user.Email,
                user.Role.ToString()
            );
        }

        public Task DeleteAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<UserDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> GetByIdAsync(string Username)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResultDto> LoginAsync(LoginRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> UpdateAsync(UpdateUserRequest request)
        {
            throw new NotImplementedException();
        }
    }


}