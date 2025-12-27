using OrderProcessing.Domain.ValueObjects;

namespace OrderProcessing.Domain.Entities
{
    public class CustomerOrder
    {
        public int OrderNumber { private set; get; }
        public float TotalPrice { private set; get; }
        public OrderStatus Status { private set; get; }
        public DateOnly OrderDate { private set; get; }
        public string Username { private set; get; }
        public CustomerOrder(
            int orderNumber,
            float totalPrice,
            OrderStatus status,
            DateOnly orderDate,
            string username)
        {
            OrderNumber = orderNumber;
            TotalPrice = totalPrice;
            Status = status;
            OrderDate = orderDate;
            Username = username;
        }
        public void ChangeStatus(OrderStatus newStatus)
        {
            Status = newStatus;
        }

    }
}