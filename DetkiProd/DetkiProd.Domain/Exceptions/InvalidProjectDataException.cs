namespace DetkiProd.Domain.Exceptions;

public class InvalidProjectDataException : DomainException
{
    public InvalidProjectDataException(string message) : base(message)
    {
    }
}
