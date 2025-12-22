namespace OrderProcessing.Api.Controllers;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Application.DTOs.Book;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Api.Models;

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
    public async Task<Results<Ok<BookDetailsDto>, NotFound<ErrorResponse>>> GetBookByISBN(string isbn)
    {
        var bookDetails = await _bookService.GetBookByISBNAsync(isbn);
        if (bookDetails == null) return TypedResults.NotFound(new ErrorResponse($"Book with ISBN {isbn} not found.", 404));
        
        return TypedResults.Ok(bookDetails);
    }

    [HttpGet]
    public async Task<Ok<IEnumerable<BookDetailsDto>>> GetAllBooks()
    {
        var books = await _bookService.GetAllBooksAsync();
        return TypedResults.Ok(books);
    }

    [HttpPost]
    public async Task<Results<Created<BookDetailsDto>, BadRequest<ErrorResponse>, Conflict<ErrorResponse>>> AddBook([FromBody] CreateBookDto dto)
    {
        var createdBook = await _bookService.CreateBookAsync(dto);
        return TypedResults.Created($"/api/books/{createdBook.ISBN}", createdBook);
    }

    [HttpPut("{isbn}")]
    public async Task<Results<NoContent, NotFound<ErrorResponse>, BadRequest<ErrorResponse>>> UpdateBook(string isbn, [FromBody] UpdateBookDto dto)
    {
        await _bookService.UpdateBookAsync(isbn, dto);
        return TypedResults.NoContent();
    }

    [HttpDelete("{isbn}")]
    public async Task<Results<NoContent, NotFound<ErrorResponse>>> DeleteBook(string isbn)
    {
        await _bookService.DeleteBookAsync(isbn);
        return TypedResults.NoContent();
    }

    [HttpGet("search")]
    public async Task<Ok<IEnumerable<BookDetailsDto>>> SearchBooks([FromQuery] SearchBooksQueryDto query)
    {
        var books = await _bookService.SearchBooksAsync(query);
        return TypedResults.Ok(books);
    }
}