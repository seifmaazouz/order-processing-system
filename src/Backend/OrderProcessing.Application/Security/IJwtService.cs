using OrderProcessing.Domain.ValueObjects;

namespace OrderProcessing.Application.Security
{
    public interface IJwtService
    {
        string GenerateToken(string username, UserTypes role);
        string GetUsernameFromToken(string token);
        UserTypes GetRoleFromToken(string token);
    }
}