namespace OrderProcessing.Application.Exceptions;

public class DuplicateResourceException : Exception
{
    public DuplicateResourceException(string message) : base(message)
    {
    }

    public DuplicateResourceException(string resourceName, string identifier) 
        : base($"{resourceName} with identifier '{identifier}' already exists.")
    {
    }
}
