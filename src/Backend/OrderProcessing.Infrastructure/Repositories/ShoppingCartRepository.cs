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
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var sql =
        """
            SELECT sc.cartid as CartId, sc.custname as CustName, ci.isbn as ISBN, ci.quantity as Quantity, ci.unitprice as UnitPrice
            FROM shoppingcart sc
            LEFT JOIN cartitem ci ON sc.cartid = ci.cartid
            WHERE sc.custname = @Username
        """;

        var cartItems = new List<CartItemReadModel>();
        int? cartId = null;
        string? custNameResult = null;

        var result = await connection.QueryAsync<dynamic>(sql, new { Username = username });
        foreach (var row in result)
        {
            // Handle both PascalCase and camelCase from Dapper
            cartId ??= row.CartId ?? row.cartid ?? row.CartID;
            custNameResult ??= row.CustName ?? row.custname;
            var isbn = row.ISBN ?? row.isbn;
            var quantity = row.Quantity ?? row.quantity;
            var unitPrice = row.UnitPrice ?? row.unitprice;
            
            if (isbn != null)
            {
                cartItems.Add(new CartItemReadModel((string)isbn, (int)quantity, (decimal)unitPrice));
            }
        }
        if (cartId == null || custNameResult == null)
            return null;
        return new ShoppingCartReadModel(cartId.Value, custNameResult, cartItems);
    }

    public async Task<int> CreateCartAsync(string username)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var sql =
        """
            INSERT INTO shoppingcart (custname)
            VALUES (@Username)
            RETURNING cartid
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
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var sql =
        """
            INSERT INTO cartitem (isbn, cartid, quantity, unitprice)
            VALUES (@ISBN, @CartId, @Quantity, @UnitPrice)
            ON CONFLICT (isbn, cartid) 
            DO UPDATE SET quantity = cartitem.quantity + @Quantity
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
            UPDATE cartitem
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
            DELETE FROM cartitem
            WHERE cartid = @CartId AND isbn = @ISBN
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
            DELETE FROM cartitem
            WHERE cartid = @CartId
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