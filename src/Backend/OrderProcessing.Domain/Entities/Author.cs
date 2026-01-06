namespace OrderProcessing.Domain.Entities
{
    public class Author // PK (ISBN, AuthorName)
    {
        public string ISBN { private set; get; } = null!; // Partial key & Foreign key to Book
        public string AuthorName { private set; get; } = null!; // Parial key

        // Navigation property (many-to-many)
        public Book Book { private set; get; } = null!;

        private Author() { } // For Dapper
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