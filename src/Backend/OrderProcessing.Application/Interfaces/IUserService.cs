using OrderProcessing.Application.DTOs.Requests;

namespace OrderProcessing.Application.Interfaces
{
    public interface IUserService
    {
        
        Task ChangePasswordAsync(ChangePasswordRequest request);
        
    }
}