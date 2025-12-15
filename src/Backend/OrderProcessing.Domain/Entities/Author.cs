namespace OrderProcessing.Domain.Entities
{
    public class Author // PK (ISBN, AuthorName)
    {
        public string ISBN { private set; get; } // Partial key & Foreign key to Book
        public string AuthorName {private set; get; } // Parial key

        // Navigation property (many-to-many)
        public Book Book { private set; get; }

        internal Author(Book book, string authorName)
        {
            Book = book ?? throw new ArgumentNullException(nameof(book));

            if (string.IsNullOrWhiteSpace(authorName))
                throw new ArgumentException("Author name cannot be empty");

            ISBN = book.ISBN;
            AuthorName = authorName;
        } 
    }
}