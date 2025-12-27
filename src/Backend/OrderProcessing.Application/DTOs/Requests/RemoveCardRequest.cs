namespace OrderProcessing.Application.DTOs.Requests
{
    public record RemoveCardRequest(
        string Token,
        string CardNumber
    );
}
