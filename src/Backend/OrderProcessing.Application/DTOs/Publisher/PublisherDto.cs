namespace OrderProcessing.Application.DTOs.Publisher;

public record PublisherDto(
    int PubID,
    string PubName,
    string Address,
    string PhoneNumber
);

