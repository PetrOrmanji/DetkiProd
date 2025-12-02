using DetkiProd.Domain.Repositories;

namespace DetkiProd.Infrastructure.Persistence.Repositories;

public abstract class DetkiProdBaseRepository : IRepository
{
    public DetkiProdContext Context { get; }

    public DetkiProdBaseRepository(DetkiProdContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task SaveAsync()
    {
        await Context.SaveChangesAsync();
    }
}
