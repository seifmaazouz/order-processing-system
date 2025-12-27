namespace OrderProcessing.Domain.Entities
{
    public class AdminOrderItem
    {
        public string ISBN { get; private set; }
        public int OrderNumber { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }

        public AdminOrderItem(string isbn, int orderNumber, int quantity, decimal unitPrice)
        {
            ISBN = isbn;
            OrderNumber = orderNumber;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
    }
}
