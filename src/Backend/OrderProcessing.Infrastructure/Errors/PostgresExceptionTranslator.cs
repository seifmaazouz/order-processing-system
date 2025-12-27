using Npgsql;
using OrderProcessing.Domain.Exceptions;

public static class PostgresExceptionTranslator
{
    public static Exception Translate(PostgresException ex)
    {
        return ex.SqlState switch
        {
            // Integrity constraint violations
            "23000" => new DataConstraintException("A database constraint was violated."),
            "23001" => new DataConstraintException("Restriction violation: Cannot delete because related records exist."),
            "23502" => TranslateNotNullConstraint(ex),
            "23503" => TranslateForeignKeyConstraint(ex),
            "23505" => TranslateUniqueConstraint(ex),
            "23514" => TranslateCheckConstraint(ex),
            
            // Data type errors
            "22001" => new DataConstraintException($"Text value is too long for column: {ex.ColumnName ?? "unknown"}"),
            "22003" => new DataConstraintException("Numeric value is out of valid range."),
            "22007" => new DataConstraintException("Invalid date/time format provided."),
            "22008" => new DataConstraintException("Date/time value is out of range."),
            "22012" => new DataConstraintException("Division by zero is not allowed."),
            "22P02" => new DataConstraintException("Invalid input format for the data type."),
            
            // Transaction errors
            "40001" => new DataConstraintException("Transaction failed due to concurrent updates. Please retry."),
            "40P01" => new DataConstraintException("Deadlock detected. Please retry the operation."),
            
            // Database object errors
            "42P01" => new DataConstraintException($"Database table does not exist: {ex.TableName ?? "unknown"}"),
            "42703" => new DataConstraintException($"Column does not exist: {ex.ColumnName ?? "unknown"}"),
            "42883" => new DataConstraintException("The requested database function does not exist."),
            "42P07" => new DataConstraintException("Database object already exists."),
            
            // Custom raised exceptions (from triggers)
            "P0001" => new DataConstraintException(ex.MessageText),
            
            _ => ex
        };
    }

    private static Exception TranslateNotNullConstraint(PostgresException ex)
    {
        var columnName = ex.ColumnName ?? "unknown field";
        var tableName = ex.TableName?.ToLower();
        
        var friendlyName = (tableName, columnName) switch
        {
            ("book", "title") => "Book title",
            ("book", "quantity") => "Book quantity",
            ("book", "threshold") => "Reorder threshold",
            ("book", "category") => "Book category",
            ("book", "pubid") => "Publisher",
            ("user", "username") => "Username",
            ("user", "password") => "Password",
            ("user", "firstname") => "First name",
            ("user", "lastname") => "Last name",
            ("user", "role") => "User role",
            ("order", "orderdate") => "Order date",
            ("order", "totalprice") => "Total price",
            ("order", "custname") => "Customer name",
            ("order", "pubid") => "Publisher",
            ("creditcard", "expirydate") => "Expiry date",
            ("shoppingcart", "custname") => "Customer name",
            ("publisher", "pubname") => "Publisher name",
            _ => columnName
        };

        return new DataConstraintException($"{friendlyName} is required.");
    }

    private static Exception TranslateCheckConstraint(PostgresException ex)
    {
        var constraintName = ex.ConstraintName?.ToLower() ?? "";
        
        // Check for enum type constraints
        if (constraintName.Contains("category") || ex.MessageText.Contains("category_enum"))
        {
            return new DataConstraintException(
                "Invalid category. Must be one of: Science, Art, Religion, History, Geography."
            );
        }
        
        if (constraintName.Contains("status") || ex.MessageText.Contains("order_status_enum"))
        {
            return new DataConstraintException(
                "Invalid order status. Must be one of: Confirmed, Pending, Canceled."
            );
        }
        
        if (constraintName.Contains("role") || ex.MessageText.Contains("role_enum"))
        {
            return new DataConstraintException(
                "Invalid role. Must be either: Customer, Admin."
            );
        }

        // Generic check constraint violations
        if (constraintName.Contains("quantity") || constraintName.Contains("qty"))
        {
            return new DataConstraintException("Quantity must be a positive number.");
        }
        
        if (constraintName.Contains("price"))
        {
            return new DataConstraintException("Price must be a positive value.");
        }
        
        if (constraintName.Contains("threshold"))
        {
            return new DataConstraintException("Threshold must be a positive number.");
        }

        return new DataConstraintException(
            ex.MessageText ?? "Invalid value supplied (business rule violation)."
        );
    }

    private static Exception TranslateUniqueConstraint(PostgresException ex)
    {
        var tableName = ex.TableName?.ToLower();
        var constraintName = ex.ConstraintName?.ToLower() ?? "";
        
        var message = tableName switch
        {
            "book" when constraintName.Contains("pkey") || constraintName.Contains("isbn") 
                => "A book with this ISBN already exists.",
            "publisher" when constraintName.Contains("pkey") || constraintName.Contains("pubid") 
                => "A publisher with this ID already exists.",
            "publisher" when constraintName.Contains("pubname") 
                => "A publisher with this name already exists.",
            "user" when constraintName.Contains("pkey") || constraintName.Contains("username") 
                => "A user with this username already exists.",
            "user" when constraintName.Contains("email") 
                => "This email address is already registered.",
            "creditcard" when constraintName.Contains("pkey") || constraintName.Contains("cardnumber") 
                => "This credit card is already registered.",
            "shoppingcart" when constraintName.Contains("pkey") || constraintName.Contains("cartid") 
                => "A shopping cart with this ID already exists.",
            "order" when constraintName.Contains("pkey") || constraintName.Contains("orderid") 
                => "An order with this ID already exists.",
            "bookauthor" when constraintName.Contains("pkey") 
                => "This author is already associated with this book.",
            "orderitem" when constraintName.Contains("pkey") 
                => "This book is already in the order.",
            "cartitem" when constraintName.Contains("pkey") 
                => "This book is already in the shopping cart.",
            "cardholder" when constraintName.Contains("pkey") 
                => "This credit card is already linked to this user.",
            _ => "A record with this identifier already exists."
        };

        return new DataConstraintException(message);
    }

    private static Exception TranslateForeignKeyConstraint(PostgresException ex)
    {
        var constraintName = ex.ConstraintName?.ToLower() ?? "";
        
        var message = constraintName switch
        {
            // Book table constraints
            "book_pubid_fkey" => "The specified publisher does not exist.",
            
            // BookAuthor table constraints
            "bookauthor_isbn_fkey" => "The specified book (ISBN) does not exist.",
            
            // Order table constraints
            "order_pubid_fkey" => "The specified publisher does not exist.",
            "order_custname_fkey" => "The specified customer (username) does not exist.",
            
            // OrderItem table constraints
            "orderitem_isbn_fkey" => "The specified book (ISBN) does not exist or is being referenced.",
            "orderitem_ordernum_fkey" => "The specified order does not exist.",
            
            // CartItem table constraints
            "cartitem_isbn_fkey" => "The specified book (ISBN) does not exist. Please check the ISBN and try again.",
            "cartitem_cartid_fkey" => "The specified shopping cart does not exist.",
            
            // ShoppingCart table constraints
            "shoppingcart_custname_fkey" => "The specified customer (username) does not exist.",
            
            // CardHolder table constraints
            "cardholder_cardnumber_fkey" => "The specified credit card does not exist.",
            "cardholder_username_fkey" => "The specified user (username) does not exist.",
            
            _ when constraintName.Contains("fkey") => DetermineForeignKeyMessage(ex),
            _ => "Referenced record does not exist or record is still in use."
        };

        return new DataConstraintException(message);
    }
    
    private static string DetermineForeignKeyMessage(PostgresException ex)
    {
        var tableName = ex.TableName?.ToLower();
        
        return tableName switch
        {
            "book" => "The specified publisher does not exist.",
            "order" => "The specified customer or publisher does not exist.",
            "orderitem" => "The specified book or order does not exist.",
            "cartitem" => "The specified book or shopping cart does not exist.",
            "shoppingcart" => "The specified customer does not exist.",
            "cardholder" => "The specified credit card or user does not exist.",
            "bookauthor" => "The specified book does not exist.",
            _ => "A required related record does not exist."
        };
    }
}