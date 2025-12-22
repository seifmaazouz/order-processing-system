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
                    username,
                    email,
                    phone_number     AS PhoneNumber,
                    first_name       AS FirstName,
                    last_name        AS LastName,
                    address,
                    password_hash    AS PasswordHash,
                    role
                FROM users
                WHERE username = @Username
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
                INSERT INTO users (
                    username,
                    email,
                    phone_number,
                    first_name,
                    last_name,
                    address,
                    password_hash,
                    role
                )
                VALUES (
                    @Username,
                    @Email,
                    @PhoneNumber,
                    @FirstName,
                    @LastName,
                    @Address,
                    @PasswordHash,
                    @Role
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
                Role = (int)user.Role
            });
        }

        public async Task UpdateAsync(User user)
        {
            const string sql = """
                UPDATE users
                SET
                    email = @Email,
                    phone_number = @PhoneNumber,
                    first_name = @FirstName,
                    last_name = @LastName,
                    address = @Address,
                    password_hash = @PasswordHash,
                    role = @Role
                WHERE username = @Username
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
                Role = (int)user.Role
            });
        }

        public async Task DeleteAsync(string username)
        {
            const string sql = """
                DELETE FROM users
                WHERE username = @Username
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            await connection.ExecuteAsync(sql, new { Username = username });
        }
    }
}