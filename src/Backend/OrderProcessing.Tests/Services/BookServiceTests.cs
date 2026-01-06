using Xunit;
using FluentAssertions;
using Moq;
using OrderProcessing.Application.Services;
using OrderProcessing.Application.DTOs.Book;
using OrderProcessing.Application.Exceptions;
using OrderProcessing.Domain.Interfaces.Repositories;
using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.ValueObjects;
using OrderProcessing.Domain.Models;

namespace OrderProcessing.Tests.Services;

public class BookServiceTests
{
    private readonly Mock<IBookRepository> _mockBookRepo;
    private readonly BookService _service;

    public BookServiceTests()
    {
        _mockBookRepo = new Mock<IBookRepository>();
        _service = new BookService(_mockBookRepo.Object);
    }

    [Fact]
    public async Task CreateBook_WhenISBNExists_ThrowsDuplicateException()
    {
        // Arrange
        var dto = new CreateBookDto
        {
            ISBN = "978-0-123456-01-0",
            Title = "Test Book",
            PublicationYear = 2020,
            SellingPrice = 50.00m,
            Quantity = 100,
            Threshold = 10,
            Category = CategoryType.Science,
            PubID = 1,
            Authors = new List<string> { "Author" }
        };

        _mockBookRepo.Setup(r => r.ExistsAsync(dto.ISBN))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateResourceException>(
            () => _service.CreateBookAsync(dto)
        );
    }

    [Fact]
    public async Task CreateBook_WhenISBNTooLong_ThrowsException()
    {
        // Arrange
        var dto = new CreateBookDto
        {
            ISBN = new string('0', 18), // 18 characters, max is 17
            Title = "Test Book",
            PublicationYear = 2020,
            SellingPrice = 50.00m,
            Quantity = 100,
            Threshold = 10,
            Category = CategoryType.Science,
            PubID = 1,
            Authors = new List<string> { "Author" }
        };

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleViolationException>(
            () => _service.CreateBookAsync(dto)
        );
    }

    [Fact]
    public async Task CreateBook_WhenTitleEmpty_ThrowsException()
    {
        // Arrange
        var dto = new CreateBookDto
        {
            ISBN = "978-0-123456-01-0",
            Title = "", // Empty title
            PublicationYear = 2020,
            SellingPrice = 50.00m,
            Quantity = 100,
            Threshold = 10,
            Category = CategoryType.Science,
            PubID = 1,
            Authors = new List<string> { "Author" }
        };

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleViolationException>(
            () => _service.CreateBookAsync(dto)
        );
    }

    [Fact]
    public async Task GetBookByISBN_WhenBookExists_ReturnsBook()
    {
        // Arrange
        var isbn = "978-0-123456-01-0";
        var bookReadModel = new BookDetailsReadModel(
            isbn, "Test Book", 2020, 50.00m, 100, 10, "Science", "Publisher", "Author"
        );

        _mockBookRepo.Setup(r => r.GetBookDetailsAsync(isbn))
            .ReturnsAsync(bookReadModel);

        // Act
        var result = await _service.GetBookByISBNAsync(isbn);

        // Assert
        result.Should().NotBeNull();
        result!.ISBN.Should().Be(isbn);
        result.Title.Should().Be("Test Book");
    }

    [Fact]
    public async Task GetBookByISBN_WhenBookNotExists_ReturnsNull()
    {
        // Arrange
        var isbn = "978-0-123456-99-9";

        _mockBookRepo.Setup(r => r.GetBookDetailsAsync(isbn))
            .ReturnsAsync((BookDetailsReadModel?)null);

        // Act
        var result = await _service.GetBookByISBNAsync(isbn);

        // Assert
        result.Should().BeNull();
    }
}

