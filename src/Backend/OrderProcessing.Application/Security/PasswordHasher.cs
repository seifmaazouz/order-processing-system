using Microsoft.AspNetCore.Identity;

namespace OrderProcessing.Application.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly PasswordHasher<object> _hasher = new();

        string IPasswordHasher.HashPassword(string password)
        {
            throw new NotImplementedException();
        }

        bool IPasswordHasher.Verify(string password, string passwordHash)
        {
            throw new NotImplementedException();
        }
    }
}