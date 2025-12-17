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

        // (book:category => many-to-one relationship)
        public int CatID { private set; get; } // Foreign key 
        public Category Category { set; get; } = null!; // Navigation property (mandatory)

        // (book:publisher => many-to-one relationship)
        public int PubID { private set; get; } // Foreign key
        public Publisher Publisher { private set; get; } = null!; // Navigation property (mandatory)

        // Multi-valued attribute author
        private readonly List<Author> _authors = new List<Author>();
        public IReadOnlyCollection<Author> Authors => _authors.AsReadOnly(); // Wrapper to expose as read-only

        // For Dapper
        private Book() { } 
        
        // Full constructor (when relations are loaded)
        public Book(
            string isbn, 
            string title, 
            int publicationYear, 
            decimal sellingPrice, 
            int quantity, 
            int threshold, 
            Category category, 
            Publisher publisher)
        {
            // Validate inputs
            ValidateInputs(isbn, sellingPrice, quantity, threshold);

            // Validate mandatory relationships
            Category = category ?? throw new ArgumentNullException(nameof(category), "Category cannot be null");
            Publisher = publisher ?? throw new ArgumentNullException(nameof(publisher), "Publisher cannot be null");

            // Assign values to properties
            ISBN = isbn;
            Title = title;
            PublicationYear = publicationYear;
            SellingPrice = sellingPrice;
            Quantity = quantity;
            Threshold = threshold;

            // Set foreign keys
            CatID = category.CatID;
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
            int catID,
            int pubID)
        {
            // Validate inputs
            ValidateInputs(isbn, sellingPrice, quantity, threshold, pubID, catID);

            // Assign values to properties
            ISBN = isbn;
            Title = title;
            PublicationYear = publicationYear;
            SellingPrice = sellingPrice;
            Quantity = quantity;
            Threshold = threshold;

            // Set foreign keys
            CatID = catID;
            PubID = pubID;
        }

        // Update method
        public void UpdateDetails (string title, int publicationYear, decimal sellingPrice, int quantity, int threshold, int catID, int pubID)
        {
            // Validate inputs
            ValidateInputs(ISBN, sellingPrice, quantity, threshold, pubID, catID);

            // Update properties
            Title = title;
            PublicationYear = publicationYear;
            SellingPrice = sellingPrice;
            Quantity = quantity;
            Threshold = threshold;
            CatID = catID;
            PubID = pubID;
        }

        // Methods to manage multi-valued attribute Authors
        public void AddAuthor(string authorName)
        {
            if (_authors.Any(a => a.AuthorName == authorName))
                throw new InvalidOperationException("Duplicate author for this book");

            _authors.Add(new Author(this, authorName));
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
        private static void ValidateInputs(string isbn, decimal sellingPrice, int quantity, int threshold, int pubID = 0, int catID = 0)
        {
            if (string.IsNullOrWhiteSpace(isbn)) throw new ArgumentException("ISBN ia required");
            if (sellingPrice <= 0) throw new ArgumentException("Price must be positive");
            if (quantity < 0) throw new ArgumentException("Quantity cannot be negative");
            if (threshold < 0) throw new ArgumentException("Threshold cannot be negative");
            if (pubID < 0) throw new ArgumentException("Invalid Publisher ID");
            if (catID < 0) throw new ArgumentException("Invalid Category ID");
        }
    }
}