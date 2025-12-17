using OrderProcessing.Application.DTOs.Book;
using OrderProcessing.Domain.Models;
using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Application.Mappings;

public static class BookMappingExtensions
{
    // CreateDto -> Entity
    public static Book ToEntity(this CreateBookDto dto)
    {
        return new Book
        (
            isbn: dto.ISBN,
            title: dto.Title,
            publicationYear: dto.PublicationYear,
            sellingPrice: dto.SellingPrice,
            quantity: dto.Quantity,
            threshold: dto.Threshold,
            catID: dto.CatID,
            pubID: dto.PubID
        );
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
            Authors: model.AuthorNames
        );
    }

    // ReadModel List -> Dto List
    public static IEnumerable<BookDetailsDto> ToDtoList(this IEnumerable<BookDetailsReadModel> models)
    {
        return models.Where(m => m != null).Select(m => m.ToDto()).ToList();
    }
}