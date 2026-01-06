namespace OrderProcessing.Domain.Entities
{
    public class ShoppingCart
    {
        public int CartId { get; private set; }
        public string Username { get; private set; } = null!; // Foreign key

        // Navigation properties
        public User? User { get; private set; }
        public ICollection<CartItem> CartItems { get; private set; } = new List<CartItem>();

        // For Dapper
        private ShoppingCart() {}

        public ShoppingCart(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username is required");
            Username = username;
        }

        // Business logic methods
        public void AddItem(string isbn, int quantity, decimal unitPrice)
        {
            if (string.IsNullOrWhiteSpace(isbn)) throw new ArgumentException("ISBN is required");
            if (quantity <= 0) throw new ArgumentException("Quantity must be positive");
            if (unitPrice < 0) throw new ArgumentException("Unit price cannot be negative");
            var existing = CartItems.FirstOrDefault(i => i.ISBN == isbn);
            if (existing != null)
            {
                existing.UpdateQuantity(existing.Quantity + quantity);
            }
            else
            {
                CartItems.Add(new CartItem(CartId, isbn, quantity, unitPrice));
            }
        }

        public void UpdateItem(string isbn, int newQuantity)
        {
            var item = CartItems.FirstOrDefault(i => i.ISBN == isbn);
            if (item == null) throw new InvalidOperationException("Item not found in cart");
            item.UpdateQuantity(newQuantity);
        }

        public void RemoveItem(string isbn)
        {
            var item = CartItems.FirstOrDefault(i => i.ISBN == isbn);
            if (item == null) throw new InvalidOperationException("Item not found in cart");
            CartItems.Remove(item);
        }

        public void ValidateNotEmpty()
        {
            if (CartItems.Count == 0) throw new InvalidOperationException("Cannot checkout an empty cart");
        }
    }
}