using DetkiProd.Domain.Enums;

namespace DetkiProd.Domain.Entities;

public class DetkiProdUserRole : BaseEntity
{
    public UserRole Name { get; set; }

    public List<DetkiProdUser> Users { get; set; } = new();

    public static DetkiProdUserRole Create(UserRole role)
    {
        var newRole = new DetkiProdUserRole()
        {
            Name = role,
        };

        return newRole;
    }
}
