using Dapper;
using OrderProcessing.Domain.Models;
using OrderProcessing.Domain.Interfaces;
using OrderProcessing.Domain.Interfaces.Repositories;

namespace OrderProcessing.Infrastructure.Repositories;

public class ShoppingCartRepository : IShoppingCartRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ShoppingCartRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ShoppingCartReadModel?> GetCartByUsernameAsync(string username)
    {
        var connection = await _connectionFactory.CreateConnectionAsync();
        var sql =
        """
            SELECT sc.CartId, sc.CustName, ci.ISBN, ci.Quantity, ci.UnitPrice
            FROM "shoppingcart" sc
            LEFT JOIN "cartitem" ci ON sc.CartId = ci.CartId
            WHERE sc.CustName = @Username
        """;

        var cartItems = new List<CartItemReadModel>();
        int? cartId = null;
        string? custNameResult = null;

        var result = await connection.QueryAsync<dynamic>(sql, new { Username = username });
        foreach (var row in result)
        {
            cartId ??= row.CartId;
            custNameResult ??= row.CustName;
            if (row.ISBN != null)
            {
                cartItems.Add(new CartItemReadModel((string)row.ISBN, (int)row.Quantity, (decimal)row.UnitPrice));
            }
        }
        if (cartId == null || custNameResult == null)
            return null;
        return new ShoppingCartReadModel(cartId.Value, custNameResult, cartItems);
    }

    public async Task<int> CreateCartAsync(string username)
    {
        var connection = await _connectionFactory.CreateConnectionAsync();
        var sql =
        """
            INSERT INTO "shoppingcart" (CustName)
            VALUES (@Username)
            RETURNING CartId
        """;
        try
        {
            return await connection.QuerySingleAsync<int>(sql, new { Username = username });
        }
        catch (Npgsql.PostgresException ex)
        {
            throw PostgresExceptionTranslator.Translate(ex);
        }
    }

    public async Task AddCartItemAsync(int cartId, CartItemReadModel cartItem)
    {
        var connection = await _connectionFactory.CreateConnectionAsync();
        var sql =
        """
            INSERT INTO "cartitem" (ISBN, CartId, Quantity, UnitPrice)
            VALUES (@ISBN, @CartId, @Quantity, @UnitPrice)
            ON CONFLICT (ISBN, CartId) 
            DO UPDATE SET Quantity = "cartitem".Quantity + @Quantity
        """;
        try
        {
            await connection.ExecuteAsync(sql, new { cartItem.ISBN, CartId = cartId, cartItem.Quantity, cartItem.UnitPrice });
        }
        catch (Npgsql.PostgresException ex)
        {
            throw PostgresExceptionTranslator.Translate(ex);
        }
    }

    public async Task<int> UpdateCartItemAsync(int cartId, CartItemReadModel cartItem)
    {
        var connection = await _connectionFactory.CreateConnectionAsync();
        var sql =
        """
            UPDATE "cartitem"
            SET Quantity = @Quantity
            WHERE CartId = @CartId AND ISBN = @ISBN
        """;
        try
        {
            var affected = await connection.ExecuteAsync(sql, new { cartItem.ISBN, CartId = cartId, cartItem.Quantity });
            return affected;
        }
        catch (Npgsql.PostgresException ex)
        {
            throw PostgresExceptionTranslator.Translate(ex);
        }
    }

    public async Task RemoveCartItemAsync(int cartId, string isbn)
    {
        var connection = await _connectionFactory.CreateConnectionAsync();
        var sql =
        """
            DELETE FROM "cartitem"
            WHERE CartId = @CartId AND ISBN = @ISBN
        """;
        try
        {
            await connection.ExecuteAsync(sql, new { CartId = cartId, ISBN = isbn });
        }
        catch (Npgsql.PostgresException ex)
        {
            throw PostgresExceptionTranslator.Translate(ex);
        }
    }

    public async Task ClearCartAsync(int cartId)
    {
        var connection = await _connectionFactory.CreateConnectionAsync();
        var sql =
        """
            DELETE FROM "cartitem"
            WHERE CartId = @CartId
        """;
        try
        {
            await connection.ExecuteAsync(sql, new { CartId = cartId });
        }
        catch (Npgsql.PostgresException ex)
        {
            throw PostgresExceptionTranslator.Translate(ex);
        }
    }
}