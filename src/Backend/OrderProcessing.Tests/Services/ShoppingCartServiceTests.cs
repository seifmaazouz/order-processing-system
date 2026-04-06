using Xunit;
using FluentAssertions;
using Moq;
using OrderProcessing.Application.Services;
using OrderProcessing.Application.DTOs.ShoppingCart;
using OrderProcessing.Application.Exceptions;
using OrderProcessing.Domain.Interfaces.Repositories;
using OrderProcessing.Domain.Models;
using OrderProcessing.Domain.ValueObjects;
using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Tests.Services;

public class ShoppingCartServiceTests
{
    private readonly Mock<IShoppingCartRepository> _mockCartRepo;
    private readonly Mock<IBookRepository> _mockBookRepo;
    private readonly Mock<ICreditCardRepository> _mockCreditCardRepo;
    private readonly Mock<ICustomerOrderRepository> _mockOrderRepo;
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly ShoppingCartService _service;

    public ShoppingCartServiceTests()
    {
        _mockCartRepo = new Mock<IShoppingCartRepository>();
        _mockBookRepo = new Mock<IBookRepository>();
        _mockCreditCardRepo = new Mock<ICreditCardRepository>();
        _mockOrderRepo = new Mock<ICustomerOrderRepository>();
        _mockUserRepo = new Mock<IUserRepository>();
        _service = new ShoppingCartService(
            _mockCartRepo.Object,
            _mockBookRepo.Object,
            _mockCreditCardRepo.Object,
            _mockOrderRepo.Object,
            _mockUserRepo.Object
        );
    }

    [Fact]
    public async Task AddItemToCart_WhenBookOutOfStock_ThrowsException()
    {
        // Arrange
        var isbn = "978-0-123456-01-0";
        var username = "testuser";
        var book = new BookDetailsReadModel(
            isbn, "Test Book", 2020, 50.00m, 0, 10, "Science", "Publisher", "Author"
        );

        _mockBookRepo.Setup(r => r.GetBookDetailsAsync(isbn))
            .ReturnsAsync(book);
        _mockBookRepo.Setup(r => r.GetBookDetailsAsync(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(new Dictionary<string, BookDetailsReadModel> { { isbn, book } });

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleViolationException>(
            () => _service.AddItemToCartAsync(username, isbn)
        );
    }

    [Fact]
    public async Task AddItemToCart_WhenQuantityExceedsStock_ThrowsException()
    {
        // Arrange
        var isbn = "978-0-123456-01-0";
        var username = "testuser";
        var book = new BookDetailsReadModel(
            isbn, "Test Book", 2020, 50.00m, 5, 10, "Science", "Publisher", "Author"
        );
        var cart = new ShoppingCartReadModel(1, username, new List<CartItemReadModel>
        {
            new CartItemReadModel(isbn, 6, 50.00m) // 6 + 1 = 7 > 5 stock
        });

        _mockBookRepo.Setup(r => r.GetBookDetailsAsync(isbn))
            .ReturnsAsync(book);
        _mockBookRepo.Setup(r => r.GetBookDetailsAsync(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(new Dictionary<string, BookDetailsReadModel> { { isbn, book } });
        _mockCartRepo.Setup(r => r.GetOrCreateCartAsync(username))
            .ReturnsAsync(cart);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleViolationException>(
            () => _service.AddItemToCartAsync(username, isbn)
        );
    }

    [Fact]
    public async Task UpdateCartItem_WhenQuantityExceedsStock_ThrowsException()
    {
        // Arrange
        var isbn = "978-0-123456-01-0";
        var username = "testuser";
        var book = new BookDetailsReadModel(
            isbn, "Test Book", 2020, 50.00m, 5, 10, "Science", "Publisher", "Author"
        );
        var cart = new ShoppingCartReadModel(1, username, new List<CartItemReadModel>
        {
            new CartItemReadModel(isbn, 2, 50.00m)
        });

        _mockBookRepo.Setup(r => r.GetBookDetailsAsync(isbn))
            .ReturnsAsync(book);
        _mockBookRepo.Setup(r => r.GetBookDetailsAsync(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(new Dictionary<string, BookDetailsReadModel> { { isbn, book } });
        _mockCartRepo.Setup(r => r.GetCartByUsernameAsync(username))
            .ReturnsAsync(cart);
        _mockCartRepo.Setup(r => r.UpdateCartItemAsync(1, It.IsAny<CartItemReadModel>()))
            .ReturnsAsync(1); // Simulate successful update

        // Act & Assert - trying to update to 10 when stock is only 5
        await Assert.ThrowsAsync<BusinessRuleViolationException>(
            () => _service.UpdateCartItemAsync(username, isbn, 10)
        );
    }

    [Fact]
    public async Task Checkout_WhenCartIsEmpty_ThrowsException()
    {
        // Arrange
        var username = "testuser";
        var cart = new ShoppingCartReadModel(1, username, new List<CartItemReadModel>());
        var checkoutDto = new CheckoutDto
        {
            CardholderName = "John Doe",
            ShippingAddress = "123 Main St",
            SavedCardNumber = null,
            NewCardNumber = 4111111111111111,
            NewCardExpiryDate = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd")
        };

        _mockCartRepo.Setup(r => r.GetCartByUsernameAsync(username))
            .ReturnsAsync(cart);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleViolationException>(
            () => _service.CheckoutAsync(username, checkoutDto)
        );
    }

    [Fact]
    public async Task Checkout_WhenStockInsufficient_ThrowsException()
    {
        // Arrange
        var username = "testuser";
        var isbn = "978-0-123456-01-0";
        var cart = new ShoppingCartReadModel(1, username, new List<CartItemReadModel>
        {
            new CartItemReadModel(isbn, 10, 50.00m)
        });
        var book = new BookDetailsReadModel(
            isbn, "Test Book", 2020, 50.00m, 5, 10, "Science", "Publisher", "Author"
        );
        var checkoutDto = new CheckoutDto
        {
            CardholderName = "John Doe",
            ShippingAddress = "123 Main St",
            SavedCardNumber = null,
            NewCardNumber = 4111111111111111,
            NewCardExpiryDate = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd")
        };

        _mockCartRepo.Setup(r => r.GetCartByUsernameAsync(username))
            .ReturnsAsync(cart);
        _mockBookRepo.Setup(r => r.GetBookDetailsAsync(isbn))
            .ReturnsAsync(book);
        _mockBookRepo.Setup(r => r.GetBookDetailsAsync(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(new Dictionary<string, BookDetailsReadModel> { { isbn, book } });
        _mockCreditCardRepo.Setup(r => r.ValidateCreditCardAsync(It.IsAny<long>(), It.IsAny<DateTime>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleViolationException>(
            () => _service.CheckoutAsync(username, checkoutDto)
        );
    }
}

