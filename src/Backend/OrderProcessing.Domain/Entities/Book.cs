using OrderProcessing.Domain.ValueObjects;

namespace OrderProcessing.Domain.Entities
{
    public class Book
    {
        public string ISBN { private set; get; } = null!; // Primary key
        public string Title { private set; get; } = null!;
        public int PublicationYear { private set; get; }
        public decimal SellingPrice { private set; get; }
        public int Quantity { private set; get; }
        public int Threshold { private set; get; }
        public CategoryType Category { private set; get; } // Refactored to use CategoryType value object
        // (book:publisher => many-to-one relationship)
        public int PubID { private set; get; } // Foreign key
        public Publisher Publisher { private set; get; } = null!; // Navigation property (mandatory)

        // Multi-valued attribute author
        private readonly List<Author> _authors = new List<Author>();
        public IReadOnlyCollection<Author> Authors => _authors.AsReadOnly(); // Wrapper to expose as read-only

        // For Dapper
        private Book() { } 
        
        // Full constructor (when relations are loaded)
        public Book (
            string isbn, 
            string title, 
            int publicationYear, 
            decimal sellingPrice, 
            int quantity, 
            int threshold, 
            CategoryType category,
            Publisher publisher)
        {
            // Validate inputs
            ValidateInputs(isbn, title, publicationYear, sellingPrice, quantity, threshold, category);

            // Validate mandatory relationships
            Publisher = publisher ?? throw new ArgumentNullException(nameof(publisher), "Publisher cannot be null");

            // Assign values to properties
            ISBN = isbn;
            Title = title;
            PublicationYear = publicationYear;
            SellingPrice = sellingPrice;
            Quantity = quantity;
            Threshold = threshold;
            Category = category;

            // Set foreign keys
            PubID = publisher.PubID;
        }

        // Lightweight constructor (used for inserts / Dapper)
        public Book(
            string isbn,
            string title,
            int publicationYear,
            decimal sellingPrice,
            int quantity,
            int threshold,
            CategoryType category,
            int pubID)
        {
            // Validate inputs
            ValidateInputs(isbn, title, publicationYear, sellingPrice, quantity, threshold, category, pubID);

            // Assign values to properties
            ISBN = isbn;
            Title = title;
            PublicationYear = publicationYear;
            SellingPrice = sellingPrice;
            Quantity = quantity;
            Threshold = threshold;
            Category = category;

            // Set foreign keys
            PubID = pubID;
        }

        // Update method
        public void UpdateDetails (string title, int publicationYear, decimal sellingPrice, int quantity, int threshold, CategoryType category, int pubID)
        {
            // Validate inputs
            ValidateInputs(ISBN, title, publicationYear, sellingPrice, quantity, threshold, category, pubID);

            // Update properties
            Title = title;
            PublicationYear = publicationYear;
            SellingPrice = sellingPrice;
            Quantity = quantity;
            Threshold = threshold;
            Category = category;
            PubID = pubID;
        }

        // Methods to manage multi-valued attribute Authors
        public void AddAuthor(string authorName)
        {
            if (_authors.Any(a => a.AuthorName == authorName))
                throw new InvalidOperationException("Duplicate author for this book");

            _authors.Add(new Author(this, authorName));
        }

        public void UpdateAuthors(List<string> newAuthors)
        {
            // Clear existing authors
            _authors.Clear();

            // Add new authors
            foreach (var authorName in newAuthors)
            {
                AddAuthor(authorName);
            }
        }
        public void RemoveAuthor(string authorName)
        {
            var author = _authors.FirstOrDefault(a => a.AuthorName == authorName);
            if (author == null)
                throw new InvalidOperationException("Author not found for this book");

            _authors.Remove(author);
        }

        // Core Business Behavior
        public void IncreaseStock(int amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive", nameof(amount));

            Quantity += amount;
        }

        public void DecreaseStock(int amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive", nameof(amount));

            if (Quantity < amount)
                throw new InvalidOperationException("Insufficient stock");

            Quantity -= amount;
        }

        // Helper method to validate inputs
        private static void ValidateInputs(string isbn, string title, int publicationYear, decimal sellingPrice, int quantity, int threshold, CategoryType category, int pubID = 0)
        {
            if (string.IsNullOrWhiteSpace(isbn)) throw new ArgumentException("ISBN ia required");
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required");
            if (publicationYear < 0 || publicationYear > DateTime.Now.Year) throw new ArgumentException("Invalid publication year");
            if (sellingPrice <= 0) throw new ArgumentException("Price must be positive");
            if (quantity < 0) throw new ArgumentException("Quantity cannot be negative");
            if (threshold < 0) throw new ArgumentException("Threshold cannot be negative");
            if (!Enum.IsDefined(typeof(CategoryType), category)) throw new ArgumentException("Invalid Category");
            if (pubID < 0) throw new ArgumentException("Invalid Publisher ID");
        }
    }
}