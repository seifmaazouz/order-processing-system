namespace OrderProcessing.Domain.Entities
{
    public class OrderItem
    {
        public int Quantity{private set; get;}
        public float UnitPrice{private set; get;}
        public int OrderNumber{private set; get;}
        public int ISBN{private set; get;}
        
    }
}