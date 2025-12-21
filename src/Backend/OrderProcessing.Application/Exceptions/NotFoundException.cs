namespace OrderProcessing.Application.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string resourceName, string identifier) 
        : base($"{resourceName} with identifier '{identifier}' not found.")
    {
    }
}