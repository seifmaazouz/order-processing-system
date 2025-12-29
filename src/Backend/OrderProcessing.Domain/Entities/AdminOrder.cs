using OrderProcessing.Domain.ValueObjects;

namespace OrderProcessing.Domain.Entities
{
    public class AdminOrder
    {
        public int OrderId { get; private set; }
        public DateOnly OrderDate { get; private set; }
        public OrderStatus Status { get; private set; }
        public decimal TotalPrice { get; private set; }
        public int PubID { get; private set; }
        public string? ConfirmedBy { get; private set; }

        // Parameterless constructor for Dapper
        public AdminOrder() { }

        public AdminOrder(
            int orderId,
            DateTime orderDate,
            string status,
            decimal totalPrice,
            int pubID,
            string? confirmedBy = null)
        {
            if (orderId < 0) throw new ArgumentException("OrderId must be non-negative");
            if (totalPrice < 0) throw new ArgumentException("Total price must be non-negative");
            if (pubID < 0) throw new ArgumentException("PubID must be non-negative");
            OrderId = orderId;
            TotalPrice = totalPrice;
            PubID = pubID;
            ConfirmedBy = confirmedBy;
            OrderDate = orderDate;
            Status = status;
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
