using OrderProcessing.Application.DTOs.User;
using OrderProcessing.Application.DTOs.Requests;
using OrderProcessing.Domain.Entities;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Domain.Interfaces.Repositories;
using OrderProcessing.Application.Security;
using OrderProcessing.Application.DTOs.Order;

namespace OrderProcessing.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICreditCardRepository _creditCardRepository;
        private readonly IJwtService _jwtService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ICustomerOrderRepository _customerOrderRepository;
        private readonly IBookRepository _bookRepository;

        public UserService(
            IUserRepository userRepository,
            ICreditCardRepository creditCardRepository,
            IJwtService jwtService,
            IPasswordHasher passwordHasher,
            ICustomerOrderRepository customerOrderRepository,
            IBookRepository bookRepository)
        {
            _userRepository = userRepository;
            _creditCardRepository = creditCardRepository;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
            _customerOrderRepository = customerOrderRepository;
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<CustomerOrderDto>> GetPastOrdersAsync(string token)
        {
            var username = _jwtService.GetUsernameFromToken(token);
            if (string.IsNullOrEmpty(username))
                throw new UnauthorizedAccessException("Invalid token.");

            // Get all orders for this user
            var orders = await _customerOrderRepository.GetByUsernameAsync(username);
            
            var orderDtos = new List<CustomerOrderDto>();

            foreach (var order in orders)
            {
                // Get order items
                var orderItems = await _customerOrderRepository.GetOrderItemsAsync(order.OrderNumber);
                
                // Get book details for each item
                var itemDtos = new List<OrderItemDto>();
                foreach (var item in orderItems)
                {
                    var book = await _bookRepository.GetBookDetailsAsync(item.ISBN);
                    // If book details not found, use ISBN as title fallback
                    itemDtos.Add(new OrderItemDto(
                        item.ISBN,
                        book?.Title ?? item.ISBN,
                        item.Quantity,
                        item.UnitPrice
                    ));
                }

                orderDtos.Add(new CustomerOrderDto(
                    order.OrderNumber,
                    order.TotalPrice,
                    order.Status,
                    order.OrderDate,
                    itemDtos
                ));
            }

            return orderDtos;
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
                user.Address,
                creditCardDtos
            );
        }

        public async Task ChangePasswordAsync(string token, ChangePasswordRequest request)
        {
            // 1. Get username from token
            var username = _jwtService.GetUsernameFromToken(token);
            if (string.IsNullOrEmpty(username))
                throw new UnauthorizedAccessException("Invalid token.");

            // 2. Get user from repository
            var user = await _userRepository.GetByUserNameAsync(username);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            // 3. Verify current password
            if (!_passwordHasher.Verify(request.OldPassword, user.PasswordHash))
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

        public async Task UpdateProfileAsync(string token, UpdateUserProfileDto dto)
        {
            var username = _jwtService.GetUsernameFromToken(token);
            if (string.IsNullOrEmpty(username))
                throw new UnauthorizedAccessException("Invalid token.");

            var user = await _userRepository.GetByUserNameAsync(username);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            // Update user with new values or keep existing ones
            var updatedUser = new User(
                user.Username,
                dto.Email ?? user.Email,
                dto.PhoneNumber ?? user.PhoneNumber,
                dto.FirstName ?? user.FirstName,
                dto.LastName ?? user.LastName,
                user.PasswordHash,
                user.Role,
                dto.Address ?? user.Address
            );

            await _userRepository.UpdateAsync(updatedUser);
        }

        public async Task AddCreditCardAsync(string token, OrderProcessing.Application.DTOs.CreditCard.AddCreditCardDto dto)
        {
            var username = _jwtService.GetUsernameFromToken(token);
            if (string.IsNullOrEmpty(username))
                throw new UnauthorizedAccessException("Invalid token.");

            // Parse expiry date from string (accepts YYYY-MM-DD or ISO format)
            if (string.IsNullOrWhiteSpace(dto.ExpiryDate))
                throw new InvalidOperationException("Expiry date is required.");

            DateOnly expiryDateOnly;
            
            // Parse expiry date - only accept YYYY-MM-DD format to avoid DateTime conversion issues
            if (!DateOnly.TryParse(dto.ExpiryDate, out expiryDateOnly))
            {
                // If direct parsing fails, try to extract date from ISO string format
                if (dto.ExpiryDate.Contains('T'))
                {
                    // Extract just the date part from ISO format (YYYY-MM-DDTHH:mm:ss...)
                    var datePart = dto.ExpiryDate.Split('T')[0];
                    if (!DateOnly.TryParse(datePart, out expiryDateOnly))
                    {
                        throw new InvalidOperationException($"Invalid expiry date format: {dto.ExpiryDate}. Expected YYYY-MM-DD format.");
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Invalid expiry date format: {dto.ExpiryDate}. Expected YYYY-MM-DD format.");
                }
            }

            // Check if user already has this card
            var existingCards = await _creditCardRepository.GetUserCardsAsync(username);
            if (existingCards.Any(c => c.CardNumber == dto.CardNumber))
                throw new InvalidOperationException($"Credit card already exists for this user.");

            // Add credit card for user
            await _creditCardRepository.AddAsync(
                new CreditCard(dto.CardNumber, expiryDateOnly),
                username
            );
        }

        public async Task RemoveCreditCardAsync(RemoveCardRequest request)
        {
            // 1. Get username from token
            var username = _jwtService.GetUsernameFromToken(request.Token);
            if (string.IsNullOrEmpty(username))
                throw new UnauthorizedAccessException("Invalid token.");

            // 2. Parse card number
            if (!long.TryParse(request.CardNumber, out var cardNumLong))
                throw new ArgumentException("Card number must be numeric.");

            // 3. Check if the card exists and belongs to the user
            var userCards = await _creditCardRepository.GetUserCardsAsync(username);
            var card = userCards.FirstOrDefault(c => c.CardNumber == cardNumLong);

            if (card == null)
                throw new KeyNotFoundException("Credit card not found for this user.");

            // 4. Delete the card using repository (shared card scenario)
            await _creditCardRepository.DeleteAsync(cardNumLong, username);
        }

    }
}
