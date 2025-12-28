using Dapper;
using Npgsql;
using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.Exceptions;
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
                VALUES (@ISBN, @Title, @PublicationYear, @SellingPrice, @Quantity, @Threshold, @Category::category_enum, @PubID)
            """;

            await connection.ExecuteAsync(bookSql,
            new
            {
                book.ISBN,
                book.Title,
                book.PublicationYear,
                book.SellingPrice,
                book.Quantity,
                book.Threshold,
                Category = book.Category.ToString(),
                book.PubID
            }, transaction);

            // Insert authors into BookAuthor junction table
            var authorSql =
            """
                INSERT INTO BookAuthor (ISBN, AuthorName)
                VALUES (@ISBN, @AuthorName)
            """;

            var authors = book.Authors.Select(a => new { book.ISBN, a.AuthorName });
            await connection.ExecuteAsync(authorSql, authors, transaction);

            transaction.Commit();
        }
        catch (PostgresException ex) {
            transaction.Rollback();
            throw PostgresExceptionTranslator.Translate(ex);
        }
        catch {
            transaction.Rollback();
            throw; // rethrow other exceptions (caught by global exception middleware)
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
                    Category = @Category::category_enum,
                    PubID = @PubID
                WHERE ISBN = @ISBN
            """;

            await connection.ExecuteAsync(bookSql, new
            {
                book.ISBN,
                book.Title,
                book.PublicationYear,
                book.SellingPrice,
                book.Quantity,
                book.Threshold,
                Category = book.Category.ToString(),
                book.PubID
            }, transaction);

            // Only update authors if there are changes
            if (book.Authors != null && book.Authors.Any())
            {
                await connection.ExecuteAsync("DELETE FROM BookAuthor WHERE ISBN = @ISBN", new { book.ISBN }, transaction);
                
                var insertAuthorSql =
                """
                    INSERT INTO BookAuthor (ISBN, AuthorName)
                    VALUES (@ISBN, @AuthorName)
                """;

                var authors = book.Authors.Select(a => new { book.ISBN, a.AuthorName });
                await connection.ExecuteAsync(insertAuthorSql, authors, transaction);
            }

            transaction.Commit();
        }
        catch (PostgresException ex) {
            transaction.Rollback();
            throw PostgresExceptionTranslator.Translate(ex);
        }
        catch {
            transaction.Rollback();
            throw; // rethrow other exceptions (caught by global exception middleware)
        }
    }

    public async Task DeleteAsync(string isbn)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        try {
            var sql = "DELETE FROM Book WHERE ISBN = @ISBN";
            await connection.ExecuteAsync(sql, new { ISBN = isbn });
        }
        catch (PostgresException ex) {
            throw PostgresExceptionTranslator.Translate(ex);
        } 
        catch {
            throw; // rethrow other exceptions (caught by global exception middleware)
        }
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
                    b.Category AS CategoryName, p.PubName AS PublisherName,
                    COALESCE(STRING_AGG(ba.AuthorName, ', '), '') AS AuthorNames
                FROM Book b
                JOIN Publisher p ON b.PubID = p.PubID
                LEFT JOIN BookAuthor ba ON b.ISBN = ba.ISBN
                WHERE b.ISBN = @ISBN
                GROUP BY b.ISBN, b.Title, b.PublicationYear, b.SellingPrice, b.Quantity, b.Threshold, b.Category, p.PubName
            """;

            var bookDetails = await connection.QuerySingleOrDefaultAsync<BookDetailsReadModel>(sql, new { ISBN = isbn });
            return bookDetails;
        }

        public async Task<Dictionary<string, BookDetailsReadModel>> GetBookDetailsAsync(IEnumerable<string> isbns)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            var sql =
            """
                SELECT b.ISBN, b.Title, b.PublicationYear, b.SellingPrice, b.Quantity, b.Threshold,
                    b.Category AS CategoryName, p.PubName AS PublisherName,
                    COALESCE(STRING_AGG(ba.AuthorName, ', '), '') AS AuthorNames
                FROM Book b
                JOIN Publisher p ON b.PubID = p.PubID
                LEFT JOIN BookAuthor ba ON b.ISBN = ba.ISBN
                WHERE b.ISBN = ANY(@ISBNs)
                GROUP BY b.ISBN, b.Title, b.PublicationYear, b.SellingPrice, b.Quantity, b.Threshold, b.Category, p.PubName
            """;

            var bookDetails = await connection.QueryAsync<BookDetailsReadModel>(sql, new { ISBNs = isbns.ToArray() });
            return bookDetails.ToDictionary(b => b.ISBN);
        }

    public async Task<IEnumerable<BookDetailsReadModel>> GetAllBookDetailsAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var sql = 
        """
            SELECT b.ISBN, b.Title, b.PublicationYear, b.SellingPrice, b.Quantity, b.Threshold,
                b.Category AS CategoryName, p.PubName AS PublisherName, 
                COALESCE(STRING_AGG(ba.AuthorName, ', '), '') AS AuthorNames
            FROM Book b
            JOIN Publisher p ON b.PubID = p.PubID
            LEFT JOIN BookAuthor ba ON b.ISBN = ba.ISBN
            GROUP BY b.ISBN, b.Title, b.PublicationYear, b.SellingPrice, b.Quantity, b.Threshold, b.Category, p.PubName
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
                b.Category AS CategoryName, p.PubName AS PublisherName, 
                COALESCE(STRING_AGG(ba.AuthorName, ', '), '') AS AuthorNames
            FROM Book b
            JOIN Publisher p ON b.PubID = p.PubID
            LEFT JOIN BookAuthor ba ON b.ISBN = ba.ISBN
            WHERE b.Quantity < b.Threshold
            GROUP BY b.ISBN, b.Title, b.PublicationYear, b.SellingPrice, b.Quantity, b.Threshold, b.Category, p.PubName
        """;

        var booksBelowThreshold = await connection.QueryAsync<BookDetailsReadModel>(sql);
        return booksBelowThreshold;
    }

    public async Task<IEnumerable<BookDetailsReadModel>> SearchBooksAsync(BookSearchFilter filter)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var searchSql =
        """
            SELECT b.ISBN, b.Title, b.PublicationYear, b.SellingPrice, b.Quantity, b.Threshold,
                b.Category AS CategoryName, p.PubName AS PublisherName, 
                COALESCE(STRING_AGG(ba.AuthorName, ', '), '') AS AuthorNames
            FROM Book b
            JOIN Publisher p ON b.PubID = p.PubID
            LEFT JOIN BookAuthor ba ON b.ISBN = ba.ISBN
            WHERE 1 = 1
        """;

        // dynamic filters 
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            // ignore dashes and find all similar ISBNs
            searchSql +=
            """
                AND (
                    REPLACE(b.ISBN, '-', '') ILIKE '%' || REPLACE(@Search, '-', '') || '%'
                     OR b.Title ILIKE '%' || @Search || '%'
                )
            """;
        }
        if (!string.IsNullOrWhiteSpace(filter.Category))
        {
            searchSql += " AND b.Category = @Category::category_enum";
        }
        if (!string.IsNullOrWhiteSpace(filter.Author))
        {
            searchSql += " AND ba.AuthorName ILIKE '%' || @Author || '%'";
        }
        if (!string.IsNullOrWhiteSpace(filter.Publisher))
        {
            searchSql += " AND p.PubName ILIKE '%' || @Publisher || '%'";
        }
        
        searchSql +=
        """
            GROUP BY b.ISBN, b.Title, b.PublicationYear, b.SellingPrice, b.Quantity, b.Threshold, b.Category, p.PubName
        """;

        var searchResults = await connection.QueryAsync<BookDetailsReadModel>(searchSql, new
        {
            filter.Search,
            Category = filter.Category?.ToString(),
            filter.Author,
            filter.Publisher
        });

        return searchResults;
    }

    public async Task UpdateBookQuantityAsync(string isbn, int quantityChange)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var sql =
        """
            UPDATE Book
            SET Quantity = Quantity + @QuantityChange
            WHERE ISBN = @ISBN
        """;
        await connection.ExecuteAsync(sql, new { ISBN = isbn, QuantityChange = quantityChange });
    }

    // Batch update book quantities (for optimized checkout)
    public async Task UpdateBookQuantitiesAsync(Dictionary<string, int> quantityChanges)
    {
        if (quantityChanges.Count == 0)
            return;

        using var connection = await _connectionFactory.CreateConnectionAsync();

        var sql =
        """
            UPDATE Book
            SET Quantity = Quantity + @QuantityChange
            WHERE ISBN = @ISBN
        """;

        var parameters = quantityChanges.Select(kvp => new { ISBN = kvp.Key, QuantityChange = kvp.Value });
        await connection.ExecuteAsync(sql, parameters);
    }
}