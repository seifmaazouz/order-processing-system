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
            if (principal == null)
                throw new SecurityTokenException("Invalid token");

            // Try both 'sub' and 'nameidentifier'
            var username = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                        ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (username == null)
                throw new SecurityTokenException("Invalid token: username not found");

            return username;
        }



        public UserTypes GetRoleFromToken(string token)
        {
            var principal = GetPrincipalFromToken(token);
            if (principal == null)
                throw new SecurityTokenException("Invalid token");

            var roleClaim = principal.FindFirst(ClaimTypes.Role)?.Value
                            ?? principal.FindFirst("role")?.Value;

            if (!Enum.TryParse<UserTypes>(roleClaim, out var role))
                throw new SecurityTokenException("Invalid token: role invalid");

            return role;
        }


        // Private helper to validate and get claims principal
        private ClaimsPrincipal? GetPrincipalFromToken(string token)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var secret = jwtSettings["Secret"] ?? throw new ArgumentNullException("JwtSettings:Secret");
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
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                // Debug: print all claims
                foreach (var claim in principal.Claims)
                {
                    Console.WriteLine($"{claim.Type}: {claim.Value}");
                }

                return principal;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Token validation failed: " + ex.Message);
                return null;
            }
        }


            }
        }
