namespace OrderProcessing.Domain.Entities
{
    public class CreditCard
    {
        public long CardNumber { private set; get; }
        public DateOnly ExpiryDate { private set; get; }
        // public string UserName { private set; get; } = null!;
        
        public CreditCard(
            long cardNumber,
            DateOnly expiryDate
            // string userName
        )
        {
            CardNumber = cardNumber;
            ExpiryDate = expiryDate;
            // UserName=userName;
        }
    }
}