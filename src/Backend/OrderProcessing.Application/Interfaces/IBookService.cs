using OrderProcessing.Application.DTOs.Book;

namespace OrderProcessing.Application.Interfaces
{
    public interface IBookService
    {
        // CRUD operations
        Task<BookDetailsDto> CreateBookAsync(CreateBookDto createBookDto);
        Task<BookDetailsDto?> GetBookByISBNAsync(string isbn);
        Task<IEnumerable<BookDetailsDto>> GetAllBooksAsync();
        Task UpdateBookAsync(string isbn, UpdateBookDto updateBookDto); // isbn passed in url /{isbn} and dto in body
        Task DeleteBookAsync(string isbn);

        Task<IEnumerable<BookDetailsDto>> GetBooksBelowStockThresholdAsync();
        Task<IEnumerable<BookDetailsDto>> SearchBooksAsync(string query);
    }
}