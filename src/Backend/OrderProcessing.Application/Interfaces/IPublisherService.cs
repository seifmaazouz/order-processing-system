using OrderProcessing.Application.DTOs.Publisher;

namespace OrderProcessing.Application.Interfaces;

public interface IPublisherService
{
    Task<IEnumerable<PublisherDto>> GetAllPublishersAsync();
    Task<PublisherDto?> GetPublisherByIdAsync(int id);
}


