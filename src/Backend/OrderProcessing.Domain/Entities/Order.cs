namespace OrderProcessing.Domain.Entities
{
    public class Order
    {
        public int OrderNumber { private set; get; }
        public float TotalPrice { private set; get; }
        public string Status { private set; get; } = null!;
        public DateOnly OrderDate { private set; get; }
        public string Username { private set; get; }
        public ICollection<OrderItem> Items { private set; get; } = null!;
        public Order(
            int orderNumber,
            float totalPrice,
            string status,
            DateOnly orderDate,
            string username)
        {
            OrderNumber = orderNumber;
            TotalPrice = totalPrice;
            Status = status;
            OrderDate = orderDate;
            Username = username;
            Items = new List<OrderItem>();
        }

    }
}