namespace OrderProcessing.Domain.Entities
{
    public class CreditCard
    {
        public string CardNumber{private set;get;}
        public DateOnly ExpiryDate{private set;get;}
        public int UserId{private set;get;}
    }
}