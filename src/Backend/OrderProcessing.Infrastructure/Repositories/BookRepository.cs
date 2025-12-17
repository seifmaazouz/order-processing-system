using Dapper;
using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.Interfaces;
using OrderProcessing.Domain.Interfaces.Repositories;
using OrderProcessing.Domain.Models;

namespace OrderProcessing.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public BookRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Book?> GetByISBNAsync(string isbn)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        
        var sql = "SELECT * FROM Book WHERE ISBN = @ISBN";

        var book = await connection.QuerySingleOrDefaultAsync<Book>(sql, new { ISBN = isbn }); // Dapper will use the parameterless private constructor
        return book;
    }

    public async Task AddAsync(Book book)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        
        var sql =
        """
            INSERT INTO Book (ISBN, Title, PublicationYear, SellingPrice, Quantity, Threshold, CatID, PubID)
            VALUES (@ISBN, @Title, @PublicationYear, @SellingPrice, @Quantity, @Threshold, @CatID, @PubID)
        """;

        await connection.ExecuteAsync(sql,
            new
            {
                book.ISBN,
                book.Title,
                book.PublicationYear,
                book.SellingPrice,
                book.Quantity,
                book.Threshold,
                book.CatID,
                book.PubID
            });
    }

    public async Task UpdateAsync(Book book)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var sql =
        """
            UPDATE Book
            SET Title = @Title,
                PublicationYear = @PublicationYear,
                SellingPrice = @SellingPrice,
                Quantity = @Quantity,
                Threshold = @Threshold,
                CatID = @CatID,
                PubID = @PubID
            WHERE ISBN = @ISBN
        """;

        await connection.ExecuteAsync(sql,
            new
            {
                book.Title,
                book.PublicationYear,
                book.SellingPrice,
                book.Quantity,
                book.Threshold,
                book.CatID,
                book.PubID,
                book.ISBN
            });
    }

    public async Task DeleteAsync(string isbn)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var sql = "DELETE FROM Book WHERE ISBN = @ISBN";

        await connection.ExecuteAsync(sql, new { ISBN = isbn });
    }

    public async Task<bool> ExistsAsync(string isbn)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var sql = "SELECT 1 FROM Book WHERE ISBN = @ISBN";
        
        var result = await connection.QuerySingleOrDefaultAsync<int?>(sql, new { ISBN = isbn });
        return result.HasValue;
    }

    public async Task<BookDetailsReadModel?> GetBookDetailsAsync(string isbn)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var sql = 
        """
            SELECT b.Title, b.PublicationYear, b.SellingPrice, b.Quantity, b. Threshold
                c.CatName, p.Name AS PubName, STRING_AGG(a.Name, ', ') AS AuthorNames
            FROM Book b
            JOIN Category c ON b.CatID = c.CatID
            JOIN Publisher p ON b.PubID = p.PubID
            JOIN BookAuthor ba ON b.ISBN = ba.ISBN
            WHERE b.ISBN = @ISBN
            GROUP BY b.ISBN, c.CatName, p.Name
        """;

        var bookDetails = await connection.QuerySingleOrDefaultAsync<BookDetailsReadModel>(sql, new { ISBN = isbn });
        return bookDetails;
    }

    public async Task<IEnumerable<BookDetailsReadModel>> GetAllBookDetailsAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var sql = 
        """
            SELECT b.ISBN, b.Title, b.PublicationYear, b.SellingPrice, b.Quantity, b. Threshold,
                c.CatName, p.Name AS PubName, STRING_AGG(a.Name, ', ') AS AuthorNames
            FROM Book b
            JOIN Category c ON b.CatID = c.CatID
            JOIN Publisher p ON b.PubID = p.PubID
            JOIN BookAuthor ba ON b.ISBN = ba.ISBN
            JOIN Author a ON ba.AuthorID = a.AuthorID
            GROUP BY b.ISBN, c.CatName, p.Name
        """;

        var bookDetailsList = await connection.QueryAsync<BookDetailsReadModel>(sql);
        return bookDetailsList;
    }
    public async Task<IEnumerable<Book>> GetBooksBelowStockThresholdAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var sql = "SELECT * FROM Book WHERE Quantity < Threshold";

        var books = await connection.QueryAsync<Book>(sql);
        return books;
    }
}