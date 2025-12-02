using DetkiProd.Domain.Exceptions;

namespace DetkiProd.Domain.Entities;

public class DetkiProdUser : BaseEntity
{
    public string? Login { get; set; }
    public string? PasswordHash { get; set; }
    public Guid RoleId { get; set; }

    public long? TelegramUserId { get; set; }

    public DetkiProdUserRole Role { get; set; }

    public static DetkiProdUser CreateBaseUser(string login, string passwordHash, DetkiProdUserRole role)
    {
        if (string.IsNullOrWhiteSpace(login) ||
            string.IsNullOrWhiteSpace(passwordHash) ||
            role == null ||
            role.Id == Guid.Empty)
        {
            throw new InvalidUserDataException();
        }

        var newUser = new DetkiProdUser()
        {
            Login = login,
            PasswordHash = passwordHash,
            RoleId = role.Id
        };

        return newUser;
    }

    public static DetkiProdUser CreateTelegramUser(long telegramUserId, DetkiProdUserRole role)
    {
        if (role == null ||
            role.Id == Guid.Empty)
        {
            throw new InvalidUserDataException();
        }

        var newUser = new DetkiProdUser()
        {
            TelegramUserId = telegramUserId,
            RoleId = role.Id
        };

        return newUser;
    }

    public void UpdateUserPassword(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new InvalidUserDataException();
        }

        PasswordHash = passwordHash;
    }

    public void UpdateUserRole(DetkiProdUserRole role)
    {
        if (role == null ||
            role.Id == Guid.Empty)
        {
            throw new InvalidUserDataException();
        }

        RoleId = role.Id;
    }
}
