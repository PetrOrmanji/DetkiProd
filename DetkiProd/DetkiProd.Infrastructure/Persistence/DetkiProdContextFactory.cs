using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace DetkiProd.Infrastructure.Persistence;

internal class DetkiProdContextFactory : IDesignTimeDbContextFactory<DetkiProdContext>
{
    public DetkiProdContext CreateDbContext(string[] args)
    {
        if (args == null || !args.Any())
            throw new ArgumentException("Args to create DbContext are null or empty");

        var optionsBuilder = new DbContextOptionsBuilder<DetkiProdContext>();
        optionsBuilder.UseNpgsql(args[0],
            builder => builder.MigrationsAssembly(typeof(DetkiProdContext).Assembly.GetName().Name));

        return new DetkiProdContext(optionsBuilder.Options);
    }
}
