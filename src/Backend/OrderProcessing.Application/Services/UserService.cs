using OrderProcessing.Application.DTOs.User;
using OrderProcessing.Application.DTOs.Requests;
using OrderProcessing.Domain.Entities;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Domain.Interfaces.Repositories;
using OrderProcessing.Application.Security;

namespace OrderProcessing.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICreditCardRepository _creditCardRepository;
        private readonly IJwtService _jwtService;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(
            IUserRepository userRepository,
            ICreditCardRepository creditCardRepository,
            IJwtService jwtService,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _creditCardRepository = creditCardRepository;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
        }

        public async Task<DetailsDto> GetDetailsAsync(string token)
        {
            var username = _jwtService.GetUsernameFromToken(token);
            if (string.IsNullOrEmpty(username))
                throw new UnauthorizedAccessException("Invalid token.");

            var user = await _userRepository.GetByUserNameAsync(username);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            var creditCards = await _creditCardRepository.GetUserCardsAsync(username);
            var creditCardDtos = creditCards?.Select(c => new CreditCardDto(
                c.CardNumber,
                c.ExpiryDate.Month.ToString("D2"),
                c.ExpiryDate.Year.ToString()
            )).ToList() ?? new List<CreditCardDto>();

            return new DetailsDto(
                user.Username,
                user.FirstName,
                user.LastName,
                user.Email,
                user.PhoneNumber,
                user.Address != null ? new List<string> { user.Address } : new List<string>(),
                creditCardDtos
            );
        }

        public async Task ChangePasswordAsync(ChangePasswordRequest request)
        {
            // 1. Get username from token
            var username = _jwtService.GetUsernameFromToken(request.Token);
            if (string.IsNullOrEmpty(username))
                throw new UnauthorizedAccessException("Invalid token.");

            // 2. Get user from repository
            var user = await _userRepository.GetByUserNameAsync(username);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            // 3. Verify current password
            if (!_passwordHasher.Verify(user.PasswordHash, request.OldPassword))
                throw new UnauthorizedAccessException("Current password is incorrect.");

            // 4. Hash new password
            var newHash = _passwordHasher.HashPassword(request.NewPassword);

            // 5. Update user password and save
            var updatedUser = new User(
                user.Username,
                user.Email,
                user.PhoneNumber,
                user.FirstName,
                user.LastName,
                newHash,      // new password hash
                user.Role,
                user.Address
            );

            await _userRepository.UpdateAsync(updatedUser);
        }
        public async Task RemoveCreditCardAsync(RemoveCardRequest request)
        {
            // 1. Get username from token
            var username = _jwtService.GetUsernameFromToken(request.Token);
            if (string.IsNullOrEmpty(username))
                throw new UnauthorizedAccessException("Invalid token.");

            // 2. Check if the card exists and belongs to the user
            var userCards = await _creditCardRepository.GetUserCardsAsync(username);
            var card = userCards.FirstOrDefault(c => c.CardNumber == request.CardNumber);

            if (card == null)
                throw new KeyNotFoundException("Credit card not found for this user.");

            // 3. Delete the card using repository
            await _creditCardRepository.DeleteAsync(request.CardNumber);
        }

    }
}
