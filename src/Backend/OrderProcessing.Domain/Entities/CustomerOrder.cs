using OrderProcessing.Domain.ValueObjects;

namespace OrderProcessing.Domain.Entities
{
    public class CustomerOrder
    {
        public int OrderNumber { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; }
        public DateOnly OrderDate { get; set; }
        public string Username { get; set; } = null!;
        public string ShippingAddress { get; set; } = null!;

        public CustomerOrder() { }

        public CustomerOrder(int orderNumber, decimal totalPrice, OrderStatus status, DateOnly orderDate, string username, string shippingAddress)
        {
            if (orderNumber < 0) throw new ArgumentException("OrderNumber must be non-negative");
            if (totalPrice < 0) throw new ArgumentException("Total price must be non-negative");
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username is required");
            if (string.IsNullOrWhiteSpace(shippingAddress)) throw new ArgumentException("Shipping address is required");
            OrderNumber = orderNumber;
            TotalPrice = totalPrice;
            Status = status;
            OrderDate = orderDate;
            Username = username;
            ShippingAddress = shippingAddress;
        }
    }
}