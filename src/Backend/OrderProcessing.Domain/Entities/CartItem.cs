using System;
namespace OrderProcessing.Domain.Entities
{
    public class CartItem // PK: (ISBN, CartId)
    {
        public string ISBN { get; private set; } = null!; // Partial Primary key
        public int CartId { get; private set; } // Partial Primary key  
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }

        // Navigation properties
        public ShoppingCart? ShoppingCart { get; private set; } = null!;
        public Book? Book { get; private set; } = null!;

        // For Dapper
        private CartItem() { }

        public CartItem(int cartId, string isbn, int quantity, decimal unitPrice)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be positive");
            if (unitPrice < 0) throw new ArgumentException("Unit price cannot be negative");
            if (string.IsNullOrWhiteSpace(isbn)) throw new ArgumentException("ISBN is required");

            CartId = cartId;
            ISBN = isbn;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0) throw new ArgumentException("Quantity must be positive");
            Quantity = newQuantity;
        }

        public void UpdateUnitPrice(decimal newUnitPrice)
        {
            if (newUnitPrice < 0) throw new ArgumentException("Unit price cannot be negative");
            UnitPrice = newUnitPrice;
        }
    }
}