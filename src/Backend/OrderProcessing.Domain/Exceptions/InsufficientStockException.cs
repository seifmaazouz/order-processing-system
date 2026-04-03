namespace OrderProcessing.Domain.Exceptions;

public class InsufficientStockException : Exception
{
    public string ISBN { get; }
    public int Available { get; }
    public string? Title { get; }

    public InsufficientStockException(string isbn, int available)
        : this(isbn, available, null)
    {
    }

    public InsufficientStockException(string isbn, int available, string? title)
        : base(title == null ? $"Insufficient stock for ISBN {isbn}. Available: {available}" : $"Insufficient stock for '{title}' ({isbn}). Available: {available}")
    {
        ISBN = isbn;
        Available = available;
        Title = title;
    }
}
