namespace OrderProcessing.Domain.Entities
{
    public class CreditCard
    {
        public string CardNumber { private set; get; } = null!;
        public DateOnly ExpiryDate { private set; get; }
        public string UserName { private set; get; } = null!;
    }
}