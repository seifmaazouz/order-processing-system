using OrderProcessing.Application.DTOs.CreditCard;
using OrderProcessing.Application.DTOs.Order;
using OrderProcessing.Application.DTOs.Requests;
using OrderProcessing.Application.DTOs.User;

namespace OrderProcessing.Application.Interfaces
{
    public interface IUserService
    {
        Task ChangePasswordAsync(string token, ChangePasswordRequest request);
        Task UpdateProfileAsync(string token, UpdateUserProfileDto dto);
        Task AddCreditCardAsync(string token, AddCreditCardDto dto);
        Task RemoveCreditCardAsync(RemoveCardRequest request);
    Task<DetailsDto> GetDetailsAsync(string token);
    Task<IEnumerable<CustomerOrderDto>> GetPastOrdersAsync(string token);
    }
}