using OrderProcessing.Application.DTOs.Publisher;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Domain.Interfaces.Repositories;

namespace OrderProcessing.Application.Services;

public class PublisherService : IPublisherService
{
    private readonly IPublisherRepository _publisherRepository;

    public PublisherService(IPublisherRepository publisherRepository)
    {
        _publisherRepository = publisherRepository;
    }

    public async Task<IEnumerable<PublisherDto>> GetAllPublishersAsync()
    {
        var publishers = await _publisherRepository.GetAllAsync();
        return publishers.Select(p => new PublisherDto(
            p.PubID,
            p.PubName,
            p.Address,
            p.PhoneNumber
        ));
    }

    public async Task<PublisherDto?> GetPublisherByIdAsync(int id)
    {
        var publisher = await _publisherRepository.GetByIdAsync(id);
        return publisher == null ? null : new PublisherDto(
            publisher.PubID,
            publisher.PubName,
            publisher.Address,
            publisher.PhoneNumber
        );
    }
}


