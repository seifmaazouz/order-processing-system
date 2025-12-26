using Microsoft.AspNetCore.Identity;

namespace OrderProcessing.Application.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly PasswordHasher<object> _hasher = new();

        // Hash a plain password
        public string HashPassword(string password)
        {
            return _hasher.HashPassword(null!, password);
        }

        // Verify a password against a stored hash
        public bool Verify(string password, string passwordHash)
        {
            var result = _hasher.VerifyHashedPassword(null!, passwordHash, password);
            return result == PasswordVerificationResult.Success;
        }
    }
}
