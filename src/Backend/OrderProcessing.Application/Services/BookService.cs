using OrderProcessing.Application.DTOs.Book;
using OrderProcessing.Application.Exceptions;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Application.Mappings;
using OrderProcessing.Domain.Interfaces.Repositories;

namespace OrderProcessing.Application.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService (IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<BookDetailsDto> CreateBookAsync(CreateBookDto dto)
        {
            // Validate ISBN and Title length
            if (string.IsNullOrWhiteSpace(dto.ISBN) || dto.ISBN.Length > 17)
                throw new BusinessRuleViolationException("ISBN must be non-empty and at most 17 characters.");
            if (string.IsNullOrWhiteSpace(dto.Title) || dto.Title.Length > 100)
                throw new BusinessRuleViolationException("Title must be non-empty and at most 100 characters.");

            var exists = await _bookRepository.ExistsAsync(dto.ISBN);
            if (exists) throw new DuplicateResourceException("Book", "ISBN", dto.ISBN);

            var book = dto.ToEntity();

            await _bookRepository.AddAsync(book);

            var readModel = await _bookRepository.GetBookDetailsAsync(dto.ISBN);
            return readModel?.ToDto() ?? throw new BusinessRuleViolationException("Failed to create book record.");
        }

        public async Task<BookDetailsDto?> GetBookByISBNAsync(string isbn)
        {
            var readModel = await _bookRepository.GetBookDetailsAsync(isbn);
            return readModel?.ToDto(); // Return null if not found
        }

        public async Task<IEnumerable<BookDetailsDto>> GetAllBooksAsync()
        {
            var readModels = await _bookRepository.GetAllBookDetailsAsync();
            return readModels.ToDtoList(); // Return empty collection if no books found
        }

        public async Task UpdateBookAsync(string isbn, UpdateBookDto dto)
        {
            var book = await _bookRepository.GetByISBNAsync(isbn);
            if (book == null)
            {
                throw new NotFoundException("Book", "ISBN", isbn);
            }

            // Validate Title length if updating
            if (dto.Title != null && (string.IsNullOrWhiteSpace(dto.Title) || dto.Title.Length > 100))
                throw new BusinessRuleViolationException("Title must be non-empty and at most 100 characters.");

            // Update fields
            book.UpdateDetails(
                title: dto.Title,
                publicationYear: dto.PublicationYear,
                sellingPrice: dto.SellingPrice,
                quantity: dto.Quantity,
                threshold: dto.Threshold,
                category: dto.Category,
                pubID: dto.PubID
            );

            // Update authors if needed
            if (dto.Authors != null)
            {
                book.UpdateAuthors(dto.Authors); // Entity will throw if empty list
            }

            await _bookRepository.UpdateAsync(book);
        }
        
        public async Task DeleteBookAsync(string isbn)
        {
            var exists = await _bookRepository.ExistsAsync(isbn);

            if (!exists) throw new NotFoundException("Book", "ISBN", isbn);

            await _bookRepository.DeleteAsync(isbn);
        }

        public Task<IEnumerable<BookDetailsDto>> SearchBooksAsync(SearchBooksQueryDto query)
        {
            var filter = query.ToDomainFilter();
            var searchResults = _bookRepository.SearchBooksAsync(filter);
            return searchResults.ContinueWith(t => t.Result.ToDtoList());
        }

        public async Task<IEnumerable<BookDetailsDto>> GetBooksBelowStockThresholdAsync()
        {
            var readModels = await _bookRepository.GetBooksBelowStockThresholdAsync();
            return readModels.ToDtoList();
        }
    }
}