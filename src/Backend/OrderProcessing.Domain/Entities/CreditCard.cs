namespace OrderProcessing.Domain.Entities
{
    public class CreditCard
    {
        public long CardNumber { get; set; }
        public DateOnly ExpiryDate { get; set; }
        public string CardholderName { get; set; } = null!;

        public CreditCard() { }
        public CreditCard(long cardNumber, DateOnly expiryDate, string cardholderName)
        {
            if (expiryDate < DateOnly.FromDateTime(DateTime.Now))
                throw new ArgumentException("Cannot add expired credit card");
            if (string.IsNullOrWhiteSpace(cardholderName))
                throw new ArgumentException("Cardholder name is required");
            CardNumber = cardNumber;
            ExpiryDate = expiryDate;
            CardholderName = cardholderName;
        }
    }
}