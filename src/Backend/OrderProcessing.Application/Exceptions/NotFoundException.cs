namespace OrderProcessing.Application.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string resourceName, string identifierName, string identifierValue) 
        : base($"{resourceName} with {identifierName} '{identifierValue}' not found.")
    {
    }
}