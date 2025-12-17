# Dapper Repository Guidelines & Entity vs DTO Usage

This document provides clear guidelines for using repositories, entities, DTOs, services, and controllers in our project using Dapper. It explains when to return domain entities and when to use DTOs / projections, along with naming conventions and aggregate usage.

---

## 1️⃣ Repositories and DTOs

- **Repositories may return DTOs / ReadModels** when the intent is **read-only** (e.g., API queries, reporting, dashboards).
- DTOs can combine fields from multiple tables, flatten relations, or calculate additional fields.
- **Repositories should return Entities** when the intent is **writing or modifying state**, to apply business rules safely.

### Examples

**Read (Query / Projection / DTO)**
```csharp
Task<BookDetailsDto> GetBookDetailsAsync(string isbn);
Task<IEnumerable<BookListItemDto>> GetBookListAsync();
```

**Write (Command / Aggregate Read)**
```csharp
Task<Book> GetBookAggregateAsync(string isbn); // Read to modify
Task UpdateAsync(Book book);                  // Persist changes
```

### Repository Example
```csharp
public interface IBookRepository
{
    // Read-only projection returns DTO directly
    Task<BookDetailsDto> GetBookDetailsAsync(string isbn);

    // Read for modification returns entity
    Task<Book> GetBookAggregateAsync(string isbn);

    Task UpdateAsync(Book book);
}

public class BookRepository : IBookRepository
{
    private readonly IDbConnection _db;

    public BookRepository(IDbConnection db) => _db = db;

    public async Task<BookDetailsDto> GetBookDetailsAsync(string isbn)
    {
        var sql = @"SELECT b.ISBN, b.Title, b.SellingPrice, c.Name AS CategoryName, p.Name AS PublisherName
                    FROM Books b
                    JOIN Categories c ON b.CatID = c.CatID
                    JOIN Publishers p ON b.PubID = p.PubID
                    WHERE b.ISBN = @ISBN";
        return await _db.QuerySingleOrDefaultAsync<BookDetailsDto>(sql, new { ISBN = isbn });
    }

    public async Task<Book> GetBookAggregateAsync(string isbn)
    {
        var sql = @"SELECT * FROM Books WHERE ISBN = @ISBN";
        var book = await _db.QuerySingleOrDefaultAsync<Book>(sql, new { ISBN = isbn });
        return book!;
    }

    public async Task UpdateAsync(Book book)
    {
        var sql = @"UPDATE Books SET Quantity = @Quantity WHERE ISBN = @ISBN";
        await _db.ExecuteAsync(sql, new { book.Quantity, book.ISBN });
    }
}
```

---

## 2️⃣ Entities

Entities encapsulate business rules and state.

- Entities are **used in repositories for write operations**.
- Entities should never be returned directly to controllers or exposed externally for read-only purposes; DTOs are used instead.

---

## 3️⃣ Services and Controllers

### Service Layer
- Services coordinate **repositories** and **business logic**.
- They convert Entities into DTOs for controllers when returning read-only data.
- They call repository methods for read/write operations.

```csharp
public class BookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository) => _bookRepository = bookRepository;

    // Repository returns DTO directly, service just returns it
    public async Task<BookDetailsDto> GetBookDetailsAsync(string isbn) =>
        await _bookRepository.GetBookDetailsAsync(isbn);

    // Repository returns entity, service modifies or reads, then maps to DTO
    public async Task<BookDetailsDto> IncreaseBookStockAsync(string isbn, int amount)
    {
        var book = await _bookRepository.GetBookAggregateAsync(isbn);
        book.IncreaseStock(amount);
        await _bookRepository.UpdateAsync(book);

        return new BookDetailsDto
        {
            ISBN = book.ISBN,
            Title = book.Title,
            SellingPrice = book.SellingPrice,
            CategoryName = book.Category.Name,
            PublisherName = book.Publisher.Name
        };
    }
}
```

### Controller Layer
- Controllers call **services** and return **DTOs** to API clients.
- They **never return entities directly** for read operations.

```csharp
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly BookService _bookService;

    public BooksController(BookService bookService) => _bookService = bookService;

    [HttpGet("{isbn}")]
    public async Task<ActionResult<BookDetailsDto>> GetBook(string isbn) =>
        Ok(await _bookService.GetBookDetailsAsync(isbn));

    [HttpPost("{isbn}/increase-stock")]
    public async Task<ActionResult<BookDetailsDto>> IncreaseStock(string isbn, [FromQuery]int amount) =>
        Ok(await _bookService.IncreaseBookStockAsync(isbn, amount));
}
```

---

## 4️⃣ Aggregates and Naming

- **Aggregate** = root entity plus related entities needed for **business rules**.
- Use `GetBookAggregateAsync` for reads intended for modification.
- Queries returning only data (projections / DTOs) should be named clearly, e.g., `GetBookListAsync`, `GetBookDetailsAsync`.

---

## 5️⃣ Projections / DTOs

- DTOs can **mix data from Book and its relations** (Category, Publisher, etc.).
- They **represent what the client needs**, not the domain structure.

```csharp
public class BookDetailsDto
{
    public string ISBN { get; set; }
    public string Title { get; set; }
    public decimal SellingPrice { get; set; }
    public string CategoryName { get; set; }
    public string PublisherName { get; set; }
}
```

---

## 6️⃣ Final Mental Models

- **Repositories** return DTOs for read-only queries or entities for command operations.
- **Services** either pass DTOs from repositories or operate on entities and then map to DTOs for controllers.
- **Controllers** return DTOs to clients and never expose entities directly.

