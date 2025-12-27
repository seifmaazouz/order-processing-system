using OrderProcessing.Application.DTOs.User;
using OrderProcessing.Domain.Interfaces.Repositories;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Application.Security;
using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.ValueObjects;
using OrderProcessing.Application.Exceptions;

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
                throw new DuplicateResourceException("Username already exists");
            }
            string password=_passwordHasher.HashPassword(request.Password);
            var user = new User(
                 request.Username,
                 request.Email,
                 request.PhoneNumber,
                 request.FirstName,
                 request.LastName,
                 password,
                role: UserTypes.Customer
            );
            await _userRepository.AddAsync(user);
            return new UserDto(
                user.Username,
                user.Email,
                user.Role.ToString()
            );
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
                ExpiresAt: DateTime.UtcNow.AddMinutes(60),
                Role: user.Role.ToString() 
            );
        }
        public async Task<AuthResultDto> LoginAdminAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByUserNameAsync(request.Username);

            if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials");

            if (user.Role != UserTypes.Admin) // check enum
                throw new UnauthorizedAccessException("Only admins can login here");

            // Generate JWT
            var token = _jwtService.GenerateToken(user.Username, user.Role);

            return new AuthResultDto(
                AccessToken: token,
                ExpiresAt: DateTime.UtcNow.AddMinutes(60),
                Role: user.Role.ToString()
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

        public Task<UserDto> GetByUsernameAsync(string Username)
        {
            throw new NotImplementedException();
        }



        public Task<UserDto> UpdateAsync(UpdateUserRequest request)
        {
            throw new NotImplementedException();
        }
    }


}