using OrderProcessing.Application.DTOs.Book;
using OrderProcessing.Domain.Models;
using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Application.Mappings;

public static class BookMappingExtensions
{
    // CreateDto -> Entity
    public static Book ToEntity(this CreateBookDto dto)
    {
        var book = new Book
        (
            isbn: dto.ISBN,
            title: dto.Title,
            publicationYear: dto.PublicationYear,
            sellingPrice: dto.SellingPrice,
            quantity: dto.Quantity,
            threshold: dto.Threshold,
            category: dto.Category,
            pubID: dto.PubID
        );

        // Add authors
        foreach (var author in dto.Authors)
        {
            book.AddAuthor(author.Trim());
        }

        return book;
    }


    // ReadModel -> DetailsDto
    public static BookDetailsDto ToDto(this BookDetailsReadModel model)
    {
        return new BookDetailsDto
        (
            ISBN: model.ISBN,
            Title: model.Title,
            Year: model.PublicationYear,
            Price: model.SellingPrice,
            Stock: model.Quantity,
            Category: model.CategoryName,
            Publisher: model.PublisherName,
            Authors: model.AuthorNames?.Split(",").Select(a => a.Trim()).ToList() ?? new List<string>() // Handle null case
        );
    }

    // ReadModel List -> Dto List
    public static IEnumerable<BookDetailsDto> ToDtoList(this IEnumerable<BookDetailsReadModel> models)
    {
        return models.Where(m => m != null).Select(m => m.ToDto()).ToList();
    }
}