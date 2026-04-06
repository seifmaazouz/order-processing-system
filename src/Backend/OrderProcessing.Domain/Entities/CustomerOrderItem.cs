namespace OrderProcessing.Domain.Entities
{
    public class CustomerOrderItem
    {
        public string ISBN { get; set; } = null!;
        public int OrderNum { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Title { get; set; } = null!;
    }
}
