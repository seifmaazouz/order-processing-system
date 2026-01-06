using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Domain.Interfaces.Repositories
{
    public interface IUserRepository{   
        Task<User?> GetByUserNameAsync(string UserId);
        Task AddAsync(User User);
        Task UpdateAsync(User User);
        Task DeleteAsync(string UserId);
        Task<IReadOnlyList<User>> GetAllCustomersAsync();
        Task<IReadOnlyList<User>> SearchCustomersAsync(string? username, string? firstName, string? lastName, string? email);
    }
}