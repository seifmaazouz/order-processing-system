namespace OrderProcessing.Domain.Exceptions;

public class DataConstraintException : Exception
{
    public DataConstraintException(string message) : base(message)
    {
    }
}
