namespace OrderProcessing.Api.Controllers;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Application.DTOs.Book;
using OrderProcessing.Application.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    // Errors thrown in service layer will be caught by Middleware (GlobalExceptionHandlingMiddleware)
    [HttpGet("{isbn}")]
    public async Task<Results<Ok<BookDetailsDto>, NotFound<string>>> GetBookDetails(string isbn)
    {
        var bookDetails = await _bookService.GetBookByISBNAsync(isbn);
        if (bookDetails == null) return TypedResults.NotFound($"Book with ISBN {isbn} not found.");
        
        return TypedResults.Ok(bookDetails);
    }

    [HttpGet]
    public async Task<Results<Ok<IEnumerable<BookDetailsDto>>, NotFound<string>>> GetAllBooks()
    {
        var books = await _bookService.GetAllBooksAsync();
        if (books == null || !books.Any()) return TypedResults.NotFound("No books found.");
        
        return TypedResults.Ok(books);
    }

    [HttpPost]
    public async Task<Results<Ok<BookDetailsDto>, BadRequest<string>>> AddBook([FromBody] CreateBookDto dto)
    {
        var createdBook = await _bookService.CreateBookAsync(dto);
        if (createdBook == null) return TypedResults.BadRequest("Failed to add the book.");
        
        return TypedResults.Ok(createdBook);
    }

    [HttpPut("{isbn}")]
    public async Task<Results<NoContent, NotFound>> UpdateBook(string isbn, [FromBody] UpdateBookDto dto)
    {
        await _bookService.UpdateBookAsync(isbn, dto);
        return TypedResults.NoContent();
    }

    [HttpDelete("{isbn}")]
    public async Task<Results<NoContent, NotFound>> DeleteBook(string isbn)
    {
        await _bookService.DeleteBookAsync(isbn);
        return TypedResults.NoContent();
    }
}