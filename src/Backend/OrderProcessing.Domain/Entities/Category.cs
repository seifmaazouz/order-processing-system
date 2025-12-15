using OrderProcessing.Domain.ValueObjects;

namespace OrderProcessing.Domain.Entities
{
    // Factory method pattern (I used this to enforce the domain property consistency)
    public class Category
    {
        public int CatID { private set; get; } // Primary key
        public string CatName { private set; get; }

        // Navigation property (category:book => one-to-many relationship)
        public ICollection<Book> Books { get; } = new List<Book>(); // Initialize the collection to avoid null reference issues
        
        private Category (int catID, string catName)
        {
            if (string.IsNullOrWhiteSpace(catName))
                throw new ArgumentException("Category name cannot be empty");

            CatID = catID;
            CatName = catName;
        }

        public static Category Create(CategoryType type)
        {
            int id = (int)type;
            string name = type.ToString();
            return new Category(id, name);
        }
    }
}