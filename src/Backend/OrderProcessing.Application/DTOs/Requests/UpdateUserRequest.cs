namespace OrderProcessing.Application.DTOs.Requests
{
    public record UpdateUserRequest(
        string Email,
        string Username
    );
    
}

