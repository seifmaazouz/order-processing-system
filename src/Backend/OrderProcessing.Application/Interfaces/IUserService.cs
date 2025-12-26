using OrderProcessing.Application.DTOs.User;

namespace OrderProcessing.Application.Interfaces
{
    public interface IUserService
    {
        
        Task ChangePasswordAsync(ChangePasswordRequest request);
        
    }
}