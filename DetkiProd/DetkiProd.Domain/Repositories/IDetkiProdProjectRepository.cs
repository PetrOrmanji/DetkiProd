using DetkiProd.Domain.Entities;

namespace DetkiProd.Domain.Repositories;

public interface IDetkiProdProjectRepository : IRepository
{
    Task AddAsync(DetkiProdProject project);

    Task<List<DetkiProdProject>> GetAllAsync();
    Task<DetkiProdProject?> GetByIdAsync(Guid id);

    Task DeleteByIdAsync(Guid id);
}
