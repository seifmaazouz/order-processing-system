using OrderProcessing.Application.DTOs.User;
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
        private readonly IJwtService _jwtService;
        public UserServices(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService
        )
    {
        _userRepository = userRepository;
        _passwordHasher=passwordHasher;
        _jwtService=jwtService;
    }
        public Task ChangePasswordAsync(ChangePasswordRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDto> CreateAsync(CreateUserRequest request)
        {
            var existedUser=await  _userRepository.GetByUserNameAsync(request.Username);
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

        public async Task<AuthResultDto> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByUserNameAsync(request.Username);
            
            if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials");

            // Generate JWT
            var token = _jwtService.GenerateToken(user.Username, user.Role);

            return new AuthResultDto(
                AccessToken: token,
                ExpiresAt: DateTime.UtcNow.AddMinutes(60) // hard coded
            );
        }


        public Task<UserDto> UpdateAsync(UpdateUserRequest request)
        {
            throw new NotImplementedException();
        }
    }


}