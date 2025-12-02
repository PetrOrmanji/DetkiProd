using DetkiProd.Domain.Entities;
using DetkiProd.Domain.Enums;
using DetkiProd.Domain.Exceptions;
using DetkiProd.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DetkiProd.Infrastructure.Persistence.Repositories;

public class DetkiProdUserRoleRepository(DetkiProdContext context) : DetkiProdBaseRepository(context), IDetkiProdUserRoleRepository
{
    public async Task<List<DetkiProdUserRole>> GetAllAsync(bool includeUsers = false)
    {
        return await CreateQueryToGetRoles(includeUsers).ToListAsync();
    }
    public async Task<DetkiProdUserRole?> GetByIdAsync(Guid id, bool includeUsers = false)
    {
        return await CreateQueryToGetRoles(includeUsers).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<DetkiProdUserRole?> GetByRoleAsync(UserRole userRole, bool includeUsers = false)
    {
        return await CreateQueryToGetRoles(includeUsers).FirstOrDefaultAsync(x => x.Name == userRole);
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        var userRole = await GetByIdAsync(id);
        if (userRole is null)
        {
            throw new UserRoleNotFoundException(id);
        }

        Context.UserRoles.Remove(userRole);
        await SaveAsync();
    }

    private IQueryable<DetkiProdUserRole> CreateQueryToGetRoles(bool includeUsers)
    {
        return includeUsers ? Context.UserRoles.Include(x => x.Users) : Context.UserRoles;
    }
}
