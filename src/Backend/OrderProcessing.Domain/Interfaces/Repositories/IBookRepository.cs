using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.Models;

namespace OrderProcessing.Domain.Interfaces.Repositories;

    public interface IBookRepository
    {
        // CRUD operations
        Task<Book?> GetByISBNAsync(string isbn);
        Task AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task DeleteAsync(string isbn);

        // Additional methods
        Task<BookDetailsReadModel?> GetBookDetailsAsync(string isbn);
        Task<IEnumerable<BookDetailsReadModel>> GetAllBookDetailsAsync();
        Task<IEnumerable<BookDetailsReadModel>> GetBooksBelowStockThresholdAsync();
        Task<IEnumerable<BookDetailsReadModel>> SearchBooksAsync(BookSearchFilter filter);
        Task<Dictionary<string, BookDetailsReadModel>> GetBookDetailsAsync(IEnumerable<string> isbns);

        // Uitility method to check existence before adding a new book
        Task<bool> ExistsAsync(string isbn);
        
        // Update book quantity (for checkout)
        Task UpdateBookQuantityAsync(string isbn, int quantityChange);
        Task UpdateBookQuantitiesAsync(Dictionary<string, int> quantityChanges);
    }