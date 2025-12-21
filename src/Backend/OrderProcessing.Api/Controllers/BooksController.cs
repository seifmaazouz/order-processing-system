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

    [HttpGet("{isbn}")]
    public async Task<Results<Ok<BookDetailsDto>, NotFound<string>>> GetBookDetails(string isbn)
    {
        var bookDetails = await _bookService.GetBookByISBNAsync(isbn);
        if (bookDetails == null) return TypedResults.NotFound($"Book with ISBN {isbn} not found.");
        
        return TypedResults.Ok(bookDetails);
    }

    [HttpPost]
    public async Task<Results<Ok<BookDetailsDto>, BadRequest<string>>> AddBook([FromBody] CreateBookDto dto)
    {
        var createdBook = await _bookService.CreateBookAsync(dto);
        if (createdBook == null) return TypedResults.BadRequest("Failed to add the book.");
        
        return TypedResults.Ok(createdBook);
    }

    [HttpPut("{isbn}")]
    // NoContent: on success, NotFound: book not found, BadRequest: invalid data
    // Errors thrown in service layer will be caught by Middleware (GlobalExceptionHandlingMiddleware)
    public async Task<Results<NoContent, NotFound, BadRequest>> UpdateBook(string isbn, [FromBody] UpdateBookDto dto)
    {
        await _bookService.UpdateBookAsync(isbn, dto);
        return TypedResults.NoContent();
    }
}