namespace OrderProcessing.Domain.Exceptions;

public class InsufficientStockException : Exception
{
    public record InsufficientItem(string ISBN, int Available, string? Title);

    // Single-item compatibility properties
    public string? ISBN { get; }
    public int? Available { get; }
    public string? Title { get; }

    // Aggregated items when multiple insufficiencies are present
    public IReadOnlyList<InsufficientItem>? Items { get; }

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
        Items = new List<InsufficientItem> { new InsufficientItem(isbn, available, title) };
    }

    public InsufficientStockException(IEnumerable<InsufficientItem> items)
        : base(CreateMessage(items))
    {
        var list = items?.ToList() ?? new List<InsufficientItem>();
        Items = list;
        if (list.Count == 1)
        {
            ISBN = list[0].ISBN;
            Available = list[0].Available;
            Title = list[0].Title;
        }
    }

    private static string CreateMessage(IEnumerable<InsufficientItem> items)
    {
        var list = items?.ToList() ?? new List<InsufficientItem>();
        if (!list.Any()) return "Insufficient stock";
        if (list.Count == 1)
        {
            var it = list[0];
            return it.Title == null ? $"Insufficient stock for ISBN {it.ISBN}. Available: {it.Available}" : $"Insufficient stock for '{it.Title}' ({it.ISBN}). Available: {it.Available}";
        }

        // Multi-item summary
        return "Insufficient stock for multiple items: " + string.Join(", ", list.Select(i => i.Title ?? i.ISBN + $"({i.Available})"));
    }
}
