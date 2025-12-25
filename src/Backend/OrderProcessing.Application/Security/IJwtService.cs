using OrderProcessing.Domain.ValueObjects;

namespace OrderProcessing.Application.Security
{
    public interface IJwtService
    {
        string GenerateToken(string username, UserTypes role);
    }
}