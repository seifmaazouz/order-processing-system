using OrderProcessing.Application.DTOs.Book;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Domain.Interfaces.Repositories;
using OrderProcessing.Application.Mappings;

namespace OrderProcessing.Application.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService (IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<BookDetailsDto?> CreateBookAsync(CreateBookDto dto)
        {
            var exists = await _bookRepository.ExistsAsync(dto.ISBN);
            if (exists) throw new Exception($"Book with ISBN {dto.ISBN} already exists"); // TODO: make throw exception better later (do for all methods)

            var book = dto.ToEntity();

            await _bookRepository.AddAsync(book);

            var readModel = await _bookRepository.GetBookDetailsAsync(dto.ISBN);
            return readModel?.ToDto() ?? throw new Exception("Book details not found"); // TODO: also throw exception if null (Middleware should catch all exceptions)
        }

        public async Task<BookDetailsDto?> GetBookByISBNAsync(string isbn)
        {
            var readModel = await _bookRepository.GetBookDetailsAsync(isbn);
            return readModel?.ToDto();
        }

        public async Task<IEnumerable<BookDetailsDto>> GetAllBooksAsync()
        {
            var readModels = await _bookRepository.GetAllBookDetailsAsync();
            return readModels.ToDtoList();
        }

        public async Task UpdateBookAsync(string isbn, UpdateBookDto dto)
        {
            var book = await _bookRepository.GetByISBNAsync(isbn);
            if (book == null) throw new Exception($"Book with ISBN {isbn} was not found."); // TODO: make custom exception later

            // Update fields
            book.UpdateDetails(
                title: dto.Title,
                publicationYear: dto.PublicationYear,
                sellingPrice: dto.SellingPrice,
                quantity: dto.Quantity,
                threshold: dto.Threshold,
                catID: dto.CatID,
                pubID: dto.PubID
            );

            // I did not update authors here, as it requires more complex logic (add/remove). Can be added later.
            // Need to check with frontend how they want to handle authors update. (If it's needed at all)

            await _bookRepository.UpdateAsync(book);
        }

        public Task DeleteBookAsync(string isbn)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BookDetailsDto>> SearchBooksAsync(string query)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BookDetailsDto>> GetBooksBelowStockThresholdAsync()
        {
            throw new NotImplementedException();
        }
    }
}