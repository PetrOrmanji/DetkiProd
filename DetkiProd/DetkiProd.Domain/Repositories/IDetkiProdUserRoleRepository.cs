using DetkiProd.Domain.Entities;
using DetkiProd.Domain.Enums;

namespace DetkiProd.Domain.Repositories;

public interface IDetkiProdUserRoleRepository : IRepository
{
    Task<List<DetkiProdUserRole>> GetAllAsync(bool includeUsers = false);
    Task<DetkiProdUserRole?> GetByIdAsync(Guid id, bool includeUsers = false);
    Task<DetkiProdUserRole?> GetByRoleAsync(UserRole userRole, bool includeUsers = false);

    Task DeleteByIdAsync(Guid id);
}
