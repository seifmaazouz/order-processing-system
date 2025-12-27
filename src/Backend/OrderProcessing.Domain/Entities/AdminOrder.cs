using OrderProcessing.Domain.ValueObjects;

namespace OrderProcessing.Domain.Entities
{
    public class AdminOrder
    {
        public int OrderId { get; private set; }
        public DateOnly OrderDate { get; private set; }
        public OrderStatus Status { get; private set; }
        public decimal TotalPrice { get; private set; }
        public int PublisherId { get; private set; }
        public string Username { get; private set; }

        public AdminOrder(
            int orderId,
            DateOnly orderDate,
            OrderStatus status,
            decimal totalPrice,
            int publisherId,
            string username)
        {
            OrderId = orderId;
            OrderDate = orderDate;
            Status = status;
            TotalPrice = totalPrice;
            PublisherId = publisherId;
            Username = username;
        }

        public void ChangeStatus(OrderStatus newStatus)
        {
            Status = newStatus;
        }
    }
}
