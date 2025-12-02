using DetkiProd.Domain.Entities;
using DetkiProd.Domain.Exceptions;
using DetkiProd.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DetkiProd.Infrastructure.Persistence.Repositories;

public class DetkiProdUserRepository(DetkiProdContext context) : DetkiProdBaseRepository(context), IDetkiProdUserRepository
{
    public async Task AddAsync(DetkiProdUser user)
    {
        await Context.Users.AddAsync(user);
        await SaveAsync();
    }

    public async Task<List<DetkiProdUser>> GetAllAsync()
    {
        return await CreateQueryToGetUsers().ToListAsync();
    }

    public async Task<DetkiProdUser?> GetByIdAsync(Guid id)
    {
        return await CreateQueryToGetUsers().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<DetkiProdUser?> GetByLoginAsync(string login)
    {
        return await CreateQueryToGetUsers().FirstOrDefaultAsync(x => x.Login == login);
    }

    public async Task<DetkiProdUser?> GetByTelegramUserIdAsync(long telegramUserId)
    {
        return await CreateQueryToGetUsers().FirstOrDefaultAsync(x => x.TelegramUserId == telegramUserId);
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        if (user is null)
        {
            throw new UserNotFoundException(id);
        }

        Context.Users.Remove(user);
        await SaveAsync();
    }

    private IQueryable<DetkiProdUser> CreateQueryToGetUsers()
    {
        return Context.Users.Include(x => x.Role);
    }
}
