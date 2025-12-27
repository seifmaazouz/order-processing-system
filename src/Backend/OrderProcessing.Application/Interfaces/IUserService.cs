using OrderProcessing.Application.DTOs.Requests;
using OrderProcessing.Application.DTOs.User;
using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Application.Interfaces
{
    public interface IUserService
    {
        
        Task ChangePasswordAsync(ChangePasswordRequest request);
        Task RemoveCreditCardAsync(RemoveCardRequest request);
        Task<DetailsDto> GetDetailsAsync(string token);
        
    }
}