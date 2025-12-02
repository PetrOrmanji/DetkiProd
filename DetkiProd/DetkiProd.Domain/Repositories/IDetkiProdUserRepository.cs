using DetkiProd.Domain.Entities;

namespace DetkiProd.Domain.Repositories;

public interface IDetkiProdUserRepository : IRepository
{
    Task AddAsync(DetkiProdUser user);

    Task<List<DetkiProdUser>> GetAllAsync();
    Task<DetkiProdUser?> GetByIdAsync(Guid id);
    Task<DetkiProdUser?> GetByTelegramUserIdAsync(long telegramUserId);
    Task<DetkiProdUser?> GetByLoginAsync(string login);

    Task DeleteByIdAsync(Guid id);
}
