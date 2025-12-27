namespace OrderProcessing.Domain.Entities
{
    public class ShoppingCart
    {
        public int CartId { get; private set; }
        public string Username { get; private set; } = string.Empty; // Foreign key

        // Navigation properties
        public User? User { get; private set; }
        public ICollection<CartItem> CartItems { get; private set; } = [];

        // For Dapper
        private ShoppingCart() {}

        public ShoppingCart(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username is required");
            Username = username;
        }
    }
}