namespace DetkiProd.Domain.Exceptions;

public class UserAlreadyExistsException : DomainException
{
    public UserAlreadyExistsException(string login) : base($"User with login '{login}' already exists.")
    {
    }

    public UserAlreadyExistsException(long telegramUserId) : base($"User with telegram user id '{telegramUserId}' already exists.")
    {
    }
}
