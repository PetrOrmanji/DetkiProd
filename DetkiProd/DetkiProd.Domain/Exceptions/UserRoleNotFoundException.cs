using DetkiProd.Domain.Enums;

namespace DetkiProd.Domain.Exceptions;

public class UserRoleNotFoundException : DomainException
{
    public UserRoleNotFoundException(Guid id) : base($"User role with id=[{id}] not found.")
    {
    }

    public UserRoleNotFoundException(UserRole userRole) : base($"User role with name=[{userRole}] not found.")
    {
    }
}
