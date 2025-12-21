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
        using var transaction = connection.BeginTransaction();

        try {
            var bookSql =
            """
                INSERT INTO Book (ISBN, Title, PublicationYear, SellingPrice, Quantity, Threshold, Category, PubID)
                VALUES (@ISBN, @Title, @PublicationYear, @SellingPrice, @Quantity, @Threshold, @Category, @PubID)
            """;

            await connection.ExecuteAsync(bookSql, book, transaction);

            // Insert authors into BookAuthor junction table
            var authorSql =
            """
                INSERT INTO BookAuthor (ISBN, AuthorName)
                VALUES (@ISBN, @AuthorName)
            """;

            var authors = book.Authors.Select(a => new { book.ISBN, a.AuthorName });
            await connection.ExecuteAsync(authorSql, authors, transaction);

            transaction.Commit();
        } catch {
            transaction.Rollback();
            throw; // Rethrow the exception to be handled by the calling code (Middleware at the end)
        }
    }

    public async Task UpdateAsync(Book book)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try {

            // update book details (excluding authors)
            var bookSql =
            """
                UPDATE Book
                SET Title = @Title,
                    PublicationYear = @PublicationYear,
                    SellingPrice = @SellingPrice,
                    Quantity = @Quantity,
                    Threshold = @Threshold,
                    Category = @Category,
                    PubID = @PubID
                WHERE ISBN = @ISBN
            """;

            await connection.ExecuteAsync(bookSql, book, transaction);

            // Only update authors if there are changes
            if (book.Authors != null && book.Authors.Any())
            {
                // Delete existing authors
                var deleteAuthorSql = "DELETE FROM BookAuthor WHERE ISBN = @ISBN";
                await connection.ExecuteAsync(deleteAuthorSql, new { book.ISBN }, transaction);

                // Insert new authors
                var insertAuthorSql =
                """
                    INSERT INTO BookAuthor (ISBN, AuthorName)
                    VALUES (@ISBN, @AuthorName)
                """;

                var authors = book.Authors.Select(a => new { book.ISBN, a.AuthorName });
                await connection.ExecuteAsync(insertAuthorSql, authors, transaction);


            }
            transaction.Commit();
        } catch {
            transaction.Rollback();
            throw; // Rethrow the exception to be handled by the calling code (Middleware at the end)
        }
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
            SELECT b.ISBN, b.Title, b.PublicationYear, b.SellingPrice, b.Quantity, b.Threshold,
                b.Category AS CategoryName, p.PubName AS PublisherName, STRING_AGG(ba.AuthorName, ', ') AS AuthorNames
            FROM Book b
            JOIN Publisher p ON b.PubID = p.PubID
            JOIN BookAuthor ba ON b.ISBN = ba.ISBN
            WHERE b.ISBN = @ISBN
            GROUP BY b.ISBN, b.Category, p.PubName
        """;

        var bookDetails = await connection.QuerySingleOrDefaultAsync<BookDetailsReadModel>(sql, new { ISBN = isbn });
        return bookDetails;
    }

    public async Task<IEnumerable<BookDetailsReadModel>> GetAllBookDetailsAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var sql = 
        """
            SELECT b.ISBN, b.Title, b.PublicationYear, b.SellingPrice, b.Quantity, b.Threshold,
                b.Category AS CategoryName, p.PubName AS PublisherName, STRING_AGG(ba.AuthorName, ', ') AS AuthorNames
            FROM Book b
            JOIN Publisher p ON b.PubID = p.PubID
            JOIN BookAuthor ba ON b.ISBN = ba.ISBN
            GROUP BY b.ISBN, b.Category, p.PubName
        """;

        var bookDetailsList = await connection.QueryAsync<BookDetailsReadModel>(sql);
        return bookDetailsList;
    }
    public async Task<IEnumerable<BookDetailsReadModel>> GetBooksBelowStockThresholdAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var sql = 
        """
            SELECT b.ISBN, b.Title, b.PublicationYear, b.SellingPrice, b.Quantity, b.Threshold,
                b.Category AS CategoryName, p.PubName AS PublisherName, STRING_AGG(ba.AuthorName, ', ') AS AuthorNames
            FROM Book b
            JOIN Publisher p ON b.PubID = p.PubID
            JOIN BookAuthor ba ON b.ISBN = ba.ISBN
            WHERE b.Quantity < b.Threshold
            GROUP BY b.ISBN, b.Category, p.PubName
        """;

        var booksBelowThreshold = await connection.QueryAsync<BookDetailsReadModel>(sql);
        return booksBelowThreshold;
    }
}