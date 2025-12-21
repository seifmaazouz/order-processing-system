using System.Text.Json.Serialization;
using OrderProcessing.Domain.ValueObjects;

namespace OrderProcessing.Application.DTOs.Book
{
    public record BookDetailsDto
    (
        string ISBN,
        string Title,
        int Year,
        decimal Price,
        int Stock,
        [property: JsonConverter(typeof(JsonStringEnumConverter))] 
        CategoryType Category,
        string Publisher,
        List<string> Authors
    );
}