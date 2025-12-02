using DetkiProd.Domain.Entities;
using DetkiProd.Domain.Exceptions;
using DetkiProd.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DetkiProd.Infrastructure.Persistence.Repositories;

public class DetkiProdProjectRepository(DetkiProdContext context) : DetkiProdBaseRepository(context), IDetkiProdProjectRepository
{
    public async Task<List<DetkiProdProject>> GetAllAsync()
    {
        return await Context.Projects.ToListAsync();
    }

    public async Task<DetkiProdProject?> GetByIdAsync(Guid id)
    {
        return await Context.Projects.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(DetkiProdProject project)
    {
        await Context.Projects.AddAsync(project);
        await SaveAsync();
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        var project = await GetByIdAsync(id);
        if (project is null)
        {
            throw new ProjectNotFoundException(id);
        }

        Context.Projects.Remove(project);
        await SaveAsync();
    }
}
