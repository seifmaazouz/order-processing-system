namespace OrderProcessing.Domain.Entities
{
    public class AdminOrderItem
    {
        public string ISBN { get; set; } = null!;
        public int OrderNum { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
