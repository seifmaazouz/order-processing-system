namespace OrderProcessing.Application.DTOs.Requests
{
    public record LogoutRequest
    {
        public required string Token { get; init; }
    }
   
}   
