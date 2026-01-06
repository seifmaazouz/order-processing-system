namespace OrderProcessing.Domain.Entities;

public class CardHolder
{
    public long CardNumber { get; private set; }
    public string Username { get; private set; } = null!;

    public CardHolder(long cardNumber, string username)
    {
        CardNumber = cardNumber;
        Username = username;
    }
}