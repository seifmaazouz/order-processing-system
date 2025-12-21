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
        Task<IEnumerable<BookDetailsReadModel>> GetBooksBelowStockThresholdAsync();

        // Uitility method to check existence before adding a new book
        Task<bool> ExistsAsync(string isbn);
    }