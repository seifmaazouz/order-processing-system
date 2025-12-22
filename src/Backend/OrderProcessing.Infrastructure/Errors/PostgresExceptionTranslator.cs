using Npgsql;
using OrderProcessing.Domain.Exceptions;

public static class PostgresExceptionTranslator
{
    public static Exception Translate(PostgresException ex)
    {
        return ex.SqlState switch
        {
            // CHECK constraint violation
            "23514" => TranslateCheckConstraint(ex),
            
            // UNIQUE constraint violation
            "23505" => TranslateUniqueConstraint(ex),
            
            // FOREIGN KEY constraint violation
            "23503" => TranslateForeignKeyConstraint(ex),
            
            // NOT NULL constraint violation
            "23502" => new DataConstraintException(
                "Required value was missing."
            ),
            
            // P0001 - Custom raised exception (from triggers)
            "P0001" => new DataConstraintException(ex.MessageText),
            
            _ => ex
        };
    }

    private static Exception TranslateCheckConstraint(PostgresException ex)
    {
        // Check constraint names from the schema
        if (ex.ConstraintName?.Contains("category", StringComparison.OrdinalIgnoreCase) == true)
        {
            return new DataConstraintException(
                "Invalid category. Must be one of: Science, Art, Religion, History, Geography."
            );
        }

        return new DataConstraintException(
            "Invalid value supplied (business rule violation)."
        );
    }

    private static Exception TranslateUniqueConstraint(PostgresException ex)
    {
        var message = ex.TableName?.ToLower() switch
        {
            "book" => "A book with this ISBN already exists.",
            "publisher" => "A publisher with this information already exists.",
            "user" => "A user with this username already exists.",
            "creditcard" => "This credit card is already registered.",
            "shoppingcart" => "A shopping cart with this identifier already exists.",
            "order" => "An order with this identifier already exists.",
            _ => "A record with this identifier already exists."
        };

        return new DataConstraintException(message);
    }

    private static Exception TranslateForeignKeyConstraint(PostgresException ex)
    {
        // Foreign key constraint names from the schema
        var message = ex.ConstraintName switch
        {
            // Book table constraints
            "book_pubid_fkey" => "The specified publisher does not exist.",
            
            // BookAuthor table constraints
            "bookauthor_isbn_fkey" => "The specified book does not exist.",
            
            // Order table constraints
            "order_pubid_fkey" => "The specified publisher does not exist.",
            "order_custname_fkey" => "The specified customer does not exist.",
            
            // OrderItem table constraints
            "orderitem_isbn_fkey" => "Cannot delete book: It exists in one or more orders.",
            "orderitem_ordernum_fkey" => "The specified order does not exist.",
            
            // CartItem table constraints
            "cartitem_isbn_fkey" => "Cannot delete book: It exists in one or more shopping carts.",
            "cartitem_cartid_fkey" => "The specified shopping cart does not exist.",
            
            // ShoppingCart table constraints
            "shoppingcart_custname_fkey" => "The specified customer does not exist.",
            
            // CardHolder table constraints
            "cardholder_cardnumber_fkey" => "The specified credit card does not exist.",
            "cardholder_username_fkey" => "The specified user does not exist.",
            
            _ => "Referenced record does not exist or record is still in use."
        };

        return new DataConstraintException(message);
    }
}