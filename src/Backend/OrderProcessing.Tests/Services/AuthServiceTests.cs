using Xunit;
using FluentAssertions;
using Moq;
using OrderProcessing.Application.Services;
using OrderProcessing.Application.DTOs.Requests;
using OrderProcessing.Application.Exceptions;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Domain.Interfaces.Repositories;
using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.ValueObjects;
using OrderProcessing.Application.Security;

namespace OrderProcessing.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly Mock<IShoppingCartService> _mockCartService;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockJwtService = new Mock<IJwtService>();
        _mockCartService = new Mock<IShoppingCartService>();
        _service = new AuthService(
            _mockUserRepo.Object,
            _mockPasswordHasher.Object,
            _mockJwtService.Object,
            _mockCartService.Object
        );
    }

    [Fact]
    public async Task CreateAsync_WhenUsernameExists_ThrowsDuplicateException()
    {
        // Arrange
        var request = new CreateUserRequest(
            "existinguser",
            "Test",
            "User",
            "test@example.com",
            "+201001234567",
            "Password123!",
            "123 Main St"
        );
        var existingUser = new User(
            "existinguser",
            "test@example.com",
            "+201001234567",
            "Test",
            "User",
            "hashedpassword",
            UserTypes.Customer,
            "123 Main St"
        );

        _mockUserRepo.Setup(r => r.GetByUserNameAsync(request.Username))
            .ReturnsAsync(existingUser);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateResourceException>(
            () => _service.CreateAsync(request)
        );
    }

    [Fact]
    public async Task LoginAsync_WhenInvalidCredentials_ThrowsUnauthorizedException()
    {
        // Arrange
        var request = new LoginRequest("testuser", "wrongpassword");
        var user = new User(
            "testuser",
            "test@example.com",
            "+201001234567",
            "Test",
            "User",
            "hashedpassword",
            UserTypes.Customer,
            "123 Main St"
        );

        _mockUserRepo.Setup(r => r.GetByUserNameAsync(request.Username))
            .ReturnsAsync(user);
        _mockPasswordHasher.Setup(h => h.Verify(request.Password, user.PasswordHash))
            .Returns(false);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.LoginAsync(request)
        );
    }

    [Fact]
    public async Task LoginAsync_WhenUserNotFound_ThrowsUnauthorizedException()
    {
        // Arrange
        var request = new LoginRequest("nonexistent", "password");

        _mockUserRepo.Setup(r => r.GetByUserNameAsync(request.Username))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.LoginAsync(request)
        );
    }

    [Fact]
    public async Task LoginAsync_WhenValidCredentials_ReturnsAuthResult()
    {
        // Arrange
        var request = new LoginRequest("testuser", "correctpassword");
        var user = new User(
            "testuser",
            "test@example.com",
            "+201001234567",
            "Test",
            "User",
            "hashedpassword",
            UserTypes.Customer,
            "123 Main St"
        );
        var expectedToken = "jwt.token.here";

        _mockUserRepo.Setup(r => r.GetByUserNameAsync(request.Username))
            .ReturnsAsync(user);
        _mockPasswordHasher.Setup(h => h.Verify(request.Password, user.PasswordHash))
            .Returns(true);
        _mockJwtService.Setup(j => j.GenerateToken(user.Username, user.Role))
            .Returns(expectedToken);

        // Act
        var result = await _service.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be(expectedToken);
        result.Role.Should().Be("Customer");
    }
}

