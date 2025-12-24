using Dapper;
using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.Interfaces;
using OrderProcessing.Domain.Interfaces.Repositories;
using OrderProcessing.Domain.ValueObjects;

namespace OrderProcessing.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UserRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<User?> GetByUserNameAsync(string username)
        {
            const string sql = """
                SELECT
                    Username,
                    "Password",    
                    FirstName,    
                    LastName,        
                    ShipAddress,
                    Email,
                    PhoneNumber,     
                    "Role"
                FROM "User"
                WHERE Username = @Username
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            var result = await connection.QuerySingleOrDefaultAsync<dynamic>(
                sql,
                new { Username = username }
            );

            if (result is null)
                return null;

            return new User(
                result.username,
                result.email,
                result.phonenumber,
                result.firstname,
                result.lastname,
                result.passwordhash,
                (UserTypes)result.role,
                result.address
            );
        }

        public async Task AddAsync(User user)
        {
            const string sql = """
                INSERT INTO "User" (
                    Username,
                    "Password",    
                    FirstName,    
                    LastName,        
                    ShipAddress,
                    Email,
                    PhoneNumber,     
                    "Role"
                )
                VALUES (
                    @Username,
                    @PasswordHash,
                    @FirstName,
                    @LastName,
                    @Address,
                    @Email,
                    @PhoneNumber,
                    @Role::role_enum
                )
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            await connection.ExecuteAsync(sql, new
            {
                user.Username,
                user.Email,
                user.PhoneNumber,
                user.FirstName,
                user.LastName,
                user.Address,
                user.PasswordHash,
                Role = user.Role.ToString()
            });
        }

        public async Task UpdateAsync(User user)
        {
            const string sql = """
                UPDATE "User"
                SET
                    Email = @Email,
                    PhoneNumber = @PhoneNumber,
                    FirstName = @FirstName,
                    LastName = @LastName,
                    ShipAddress = @Address,
                    "Password" = @PasswordHash,
                    "Role" = @Role
                WHERE username = @Username
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            await connection.ExecuteAsync(sql, new
            {
                user.Email,
                user.PhoneNumber,
                user.FirstName,
                user.LastName,
                user.Address,
                user.PasswordHash,
                Role = user.Role.ToString()
            });
        }

        public async Task DeleteAsync(string username)
        {
            const string sql = """
                DELETE FROM User
                WHERE Username = @Username
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            await connection.ExecuteAsync(sql, new { Username = username });
        }
    }
}