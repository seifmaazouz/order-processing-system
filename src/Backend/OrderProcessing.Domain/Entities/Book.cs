namespace OrderProcessing.Domain.Entities
{
     public class Book
    {
        public string ISBN { private set; get; } // Primary key
        public string Title { private set; get; }
        public int PublicationYear { private set; get; }
        public decimal SellingPrice { private set; get; }
        public int Quantity { private set; get; }
        public int Threshold { private set; get; }

        // (book:category => many-to-one relationship)
        public int CatID { private set; get; } // Foreign key 
        public Category Category { set; get; }// Navigation property (mandatory)

        // (book:publisher => many-to-one relationship)
        public int PubID { private set; get; } // Foreign key
        public Publisher Publisher { private set; get; } // Navigation property (mandatory)

        public Book(
            string isbn, 
            string title, 
            int publicationYear, 
            decimal sellingPrice, 
            int quantity, 
            int threshold, 
            int catID, 
            Category category, 
            int pubID, 
            Publisher publisher)
        {
            // 1. Validate input parameters
            if (string.IsNullOrWhiteSpace(isbn) || sellingPrice <= 0 || quantity < 0 || threshold < 0)
            {
                throw new ArgumentException("Invalid data provided for Book creation (ISBN, Title, Price, Quantity, or Threshold).");
            }

            // 2. Validate mandatory relationships
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category), "Category cannot be null.");
            }
            if (publisher == null)
            {
                throw new ArgumentNullException(nameof(publisher), "Publisher cannot be null.");
            }

            // 3. Consistency checks (fk values should match the navigation properties)
            if (category.CatID != CatID)
            {
                throw new ArgumentException("Category ID does not match the provided Category entity.");
            }
            if (publisher.PubID != PubID)
            {
                throw new ArgumentException("Publisher ID does not match the provided Publisher entity.");
            }

            // 4. Assign values to properties
            ISBN = isbn;
            Title = title;
            PublicationYear = publicationYear;
            SellingPrice = sellingPrice;
            Quantity = quantity;
            Threshold = threshold;

            // Set foreign keys and navigation properties
            CatID = catID;
            Category = category;
            PubID = pubID;
            Publisher = publisher;
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
    }
}