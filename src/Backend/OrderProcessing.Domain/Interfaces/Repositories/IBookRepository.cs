using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Domain.Interfaces.Repositories;

    public interface IBookRepository
    {
        // CRUD operations
        Task<Book?> GetByISBNAsync(string isbn);
        Task AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task DeleteAsync(string isbn);

        // 3. Confirm Orders (Admin Only) 
        // Admin can place order even if quatity < threshold
        Task<IEnumerable<Book>> GetBooksBelowStockThresholdAsync();

        // Uitility method to check existence before adding a new book
        Task<bool> ExistsAsync(string isbn);
    }