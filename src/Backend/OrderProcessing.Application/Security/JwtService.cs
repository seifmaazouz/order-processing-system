using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using OrderProcessing.Domain.ValueObjects;
using OrderProcessing.Application.Security;
using Microsoft.Extensions.Configuration;

namespace OrderProcessing.Application.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(string username, UserTypes role)
        {
            var jwtSettings = _config.GetSection("JwtSettings");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(ClaimTypes.Role, role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var secret = jwtSettings["Secret"] ?? throw new ArgumentNullException("JwtSettings:Secret", "JWT secret key is not configured.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GetUsernameFromToken(string token)
        {
            var principal = GetPrincipalFromToken(token);
            return principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value 
                   ?? throw new SecurityTokenException("Invalid token: username not found");
        }

        public UserTypes GetRoleFromToken(string token)
        {
            var principal = GetPrincipalFromToken(token);
            var roleClaim = principal?.FindFirst(ClaimTypes.Role)?.Value 
                            ?? throw new SecurityTokenException("Invalid token: role not found");

            return Enum.TryParse<UserTypes>(roleClaim, out var role) 
                ? role 
                : throw new SecurityTokenException("Invalid token: role invalid");
        }

        // Private helper to validate and get claims principal
        private ClaimsPrincipal? GetPrincipalFromToken(string token)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var secret = jwtSettings["Secret"] ?? throw new ArgumentNullException("JwtSettings:Secret", "JWT secret key is not configured.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero // optional: remove default 5 min clock skew
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch
            {
                return null; // token invalid
            }
        }
    }
}
