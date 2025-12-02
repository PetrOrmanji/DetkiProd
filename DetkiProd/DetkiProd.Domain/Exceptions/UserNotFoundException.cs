namespace DetkiProd.Domain.Exceptions;

public class UserNotFoundException : DomainException
{
    public UserNotFoundException(Guid id) : base($"User with id=[{id}] not found.")
    {
    }
}