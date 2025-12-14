namespace OrderProcessing.Domain.Entities
{
    public class Order
    {
        public int OrderNumber{private set; get;}
        public float TotalPrice{private set; get;}
        public string Status{private set; get;}
        public DateOnly OrderDate{private set; get;}
        public int UserId{private set; get;}

    }
}