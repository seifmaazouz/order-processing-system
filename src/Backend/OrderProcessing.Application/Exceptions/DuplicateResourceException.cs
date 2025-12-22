namespace OrderProcessing.Application.Exceptions;

public class DuplicateResourceException : Exception
{
    public DuplicateResourceException(string message) : base(message)
    {
    }

    public DuplicateResourceException(string resourceName, string identifierName, string identifierValue) 
        : base($"{resourceName} with {identifierName} '{identifierValue}' already exists.")
    {
    }
}
