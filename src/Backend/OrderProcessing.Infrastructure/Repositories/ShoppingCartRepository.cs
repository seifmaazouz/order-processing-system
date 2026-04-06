using Dapper;
using Microsoft.Extensions.Logging;
using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.Models;
using OrderProcessing.Domain.Interfaces;
using OrderProcessing.Domain.Interfaces.Repositories;

namespace OrderProcessing.Infrastructure.Repositories;

public class ShoppingCartRepository : IShoppingCartRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<ShoppingCartRepository> _logger;

    public ShoppingCartRepository(IDbConnectionFactory connectionFactory, ILogger<ShoppingCartRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<ShoppingCartReadModel?> GetCartByUsernameAsync(string username)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        // First get the cart info
        var cartSql = """
            SELECT
                CartId,
                CustName AS Username
            FROM ShoppingCart
            WHERE CustName = @Username
        """;

        var cartInfo = await connection.QueryFirstOrDefaultAsync<(int CartId, string Username)>(cartSql, new { Username = username });
        if (cartInfo == default || cartInfo.CartId == 0)
            return null;

        int cartId = cartInfo.CartId;
        string userName = cartInfo.Username;

        // Then get the cart items
        var itemsSql =
        """
            SELECT
                CartId,
                ISBN,
                Quantity,
                UnitPrice
            FROM CartItem
            WHERE CartId = @CartId
        """;

        var cartItems = await connection.QueryAsync<CartItem>(itemsSql, new { CartId = cartId });

        // Map domain CartItem entity to CartItemReadModel for transport
        var readModels = cartItems.Select(ci => new CartItemReadModel(ci.ISBN, ci.Quantity, ci.UnitPrice)).ToList();

        return new ShoppingCartReadModel(cartId, userName, readModels);
    }

    public async Task<int> CreateCartAsync(string username)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        // First check if cart already exists (defensive check)
        var existingCartSql = "SELECT CartId FROM ShoppingCart WHERE custname = @Username";
        var existingCartId = await connection.QueryFirstOrDefaultAsync<int?>(existingCartSql, new { Username = username });
        if (existingCartId.HasValue)
        {
            return existingCartId.Value;
        }

        // Create new cart
        var insertSql =
        @"INSERT INTO ShoppingCart (custname)
            VALUES (@Username)
            RETURNING cartid AS CartId";

        try
        {
            return await connection.QuerySingleAsync<int>(insertSql, new { Username = username });
        }
        catch (Npgsql.PostgresException ex)
        {
            throw PostgresExceptionTranslator.Translate(ex);
        }
    }

    public async Task<ShoppingCartReadModel> GetOrCreateCartAsync(string username)
    {
        // Try to get existing cart first
        var existingCart = await GetCartByUsernameAsync(username);

        if (existingCart != null)
        {
            return existingCart;
        }

        // If no cart exists, create one
        var cartId = await CreateCartAsync(username);
        return new ShoppingCartReadModel(cartId, username, new List<CartItemReadModel>());
    }

    public async Task AddCartItemAsync(int cartId, CartItem cartItem)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var sql =
        """
            INSERT INTO CartItem (ISBN, CartId, Quantity, UnitPrice)
            VALUES (@ISBN, @CartId, @Quantity, @UnitPrice)
            ON CONFLICT (ISBN, CartId)
            DO UPDATE SET Quantity = CartItem.Quantity + @Quantity
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
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var sql =
        """
            UPDATE CartItem
            SET quantity = @Quantity
            WHERE cartid = @CartId AND isbn = @ISBN
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
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var sql =
        """
            DELETE FROM CartItem
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
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var sql =
        """
            DELETE FROM CartItem
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
    public async Task<int> GetCartItemCountAsync(string username)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        // Get the cart id for the user
        var cartIdSql = @"SELECT cartid AS CartId FROM ShoppingCart WHERE custname = @Username";
        var cartId = await connection.QueryFirstOrDefaultAsync<int?>(cartIdSql, new { Username = username });
        if (cartId == null)
            return 0;
        // Sum the quantities of all items in the cart
        var countSql = @"SELECT COALESCE(SUM(quantity), 0) FROM CartItem WHERE cartid = @CartId";
        var count = await connection.QueryFirstAsync<int>(countSql, new { CartId = cartId });
        return count;
    }
}