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

            var row = await connection.QuerySingleOrDefaultAsync<UserRow>(
                sql,
                new { Username = username }
            );

            if (row is null)
                return null;

            // Safely parse the role, default to Customer if null/empty/invalid
            UserTypes role = UserTypes.Customer;
            if (!string.IsNullOrWhiteSpace(row.Role))
            {
                Enum.TryParse<UserTypes>(row.Role, true, out role);
            }

            return new User(
                row.Username,
                row.Email,
                row.PhoneNumber,
                row.FirstName,
                row.LastName,
                row.Password,
                role,
                row.ShipAddress ?? string.Empty
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
                    Email,
                    PhoneNumber,     
                    ShipAddress,
                    "Role"
                )
                VALUES (
                    @Username,
                    @PasswordHash,
                    @FirstName,
                    @LastName,
                    @Email,
                    @PhoneNumber,
                    @Address,
                    @Role::role_enum
                )
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            await connection.ExecuteAsync(sql, new
            {
                user.Username,
                user.PasswordHash,
                user.FirstName,
                user.LastName,
                user.Email,
                user.PhoneNumber,
                Role = user.Role.ToString(),
                user.Address
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
                    "Role" = @Role::role_enum
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
                Role = user.Role.ToString(),
                user.Username
            });
        }

        public async Task DeleteAsync(string username)
        {
            const string sql = """
                DELETE FROM "User"
                WHERE Username = @Username
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            await connection.ExecuteAsync(sql, new { Username = username });
        }

        public async Task<IReadOnlyList<User>> GetAllCustomersAsync()
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
                WHERE "Role" = 'Customer'
                ORDER BY Username
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            var rows = await connection.QueryAsync<UserRow>(sql);

            return rows.Select(row =>
            {
                UserTypes role = UserTypes.Customer;
                if (!string.IsNullOrWhiteSpace(row.Role))
                {
                    Enum.TryParse<UserTypes>(row.Role, true, out role);
                }

                return new User(
                    row.Username,
                    row.Email,
                    row.PhoneNumber,
                    row.FirstName,
                    row.LastName,
                    row.Password,
                    role,
                    row.ShipAddress ?? string.Empty
                );
            }).ToList();
        }

        public async Task<IReadOnlyList<User>> SearchCustomersAsync(string? username, string? firstName, string? lastName, string? email)
        {
            var conditions = new List<string> { "\"Role\" = 'Customer'" };
            var parameters = new Dictionary<string, object>();

            if (!string.IsNullOrWhiteSpace(username))
            {
                conditions.Add("Username ILIKE @Username");
                parameters["Username"] = $"%{username}%";
            }

            if (!string.IsNullOrWhiteSpace(firstName))
            {
                conditions.Add("FirstName ILIKE @FirstName");
                parameters["FirstName"] = $"%{firstName}%";
            }

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                conditions.Add("LastName ILIKE @LastName");
                parameters["LastName"] = $"%{lastName}%";
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                conditions.Add("Email ILIKE @Email");
                parameters["Email"] = $"%{email}%";
            }

            var sql = $"""
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
                WHERE {string.Join(" AND ", conditions)}
                ORDER BY Username
            """;

            using var connection = await _connectionFactory.CreateConnectionAsync();

            var rows = await connection.QueryAsync<UserRow>(sql, parameters);

            return rows.Select(row =>
            {
                UserTypes role = UserTypes.Customer;
                if (!string.IsNullOrWhiteSpace(row.Role))
                {
                    Enum.TryParse<UserTypes>(row.Role, true, out role);
                }

                return new User(
                    row.Username,
                    row.Email,
                    row.PhoneNumber,
                    row.FirstName,
                    row.LastName,
                    row.Password,
                    role,
                    row.ShipAddress ?? string.Empty
                );
            }).ToList();
        }

        private record UserRow(
            string Username,
            string Password,
            string FirstName,
            string LastName,
            string? ShipAddress,
            string Email,
            string PhoneNumber,
            string? Role
        );

    }
}