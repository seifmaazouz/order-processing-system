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
        public string? ConfirmedBy { get; private set; }

        public AdminOrder(
            int orderId,
            DateTime orderDate,
            string status,
            decimal totalPrice,
            int publisherId,
            string? confirmedBy = null)
        {
            if (orderId < 0) throw new ArgumentException("OrderId must be non-negative");
            if (totalPrice < 0) throw new ArgumentException("Total price must be non-negative");
            if (publisherId < 0) throw new ArgumentException("PublisherId must be non-negative");
            OrderId = orderId;
            TotalPrice = totalPrice;
            PublisherId = publisherId;
            ConfirmedBy = confirmedBy;
            OrderDate = DateOnly.FromDateTime(orderDate.Date);
            if (!Enum.TryParse<OrderStatus>(status, true, out OrderStatus orderStatus))
                throw new ArgumentException($"Invalid order status: {status}. Valid values are: Pending, Confirmed, Canceled");
            Status = orderStatus;
        }

        public void ChangeStatus(OrderStatus newStatus)
        {
            if (!Enum.IsDefined(typeof(OrderStatus), newStatus))
                throw new ArgumentException($"Invalid order status: {newStatus}");
            Status = newStatus;
        }

        public void ConfirmBy(string adminUsername)
        {
            if (string.IsNullOrWhiteSpace(adminUsername))
                throw new ArgumentException("Admin username is required when confirming an order");
            Status = OrderStatus.Confirmed;
            ConfirmedBy = adminUsername;
        }
    }
}
