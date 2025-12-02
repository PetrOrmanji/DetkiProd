using DetkiProd.Domain.Entities;
using DetkiProd.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DetkiProd.Infrastructure.Persistence;

public class DetkiProdContext(DbContextOptions<DetkiProdContext> options) : DbContext(options)
{
    public DbSet<DetkiProdUser> Users { get; set; }
    public DbSet<DetkiProdUserRole> UserRoles { get; set; }
    public DbSet<DetkiProdProject> Projects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DetkiProdUser>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Login).IsRequired(false);
            entity.HasIndex(x => x.Login).IsUnique();

            entity.Property(x => x.PasswordHash).IsRequired(false);

            entity.Property(x => x.TelegramUserId).IsRequired(false);
            entity.HasIndex(x => x.TelegramUserId).IsUnique();

            entity.HasOne(x => x.Role).WithMany(y => y.Users).HasForeignKey(d => d.RoleId).IsRequired();
        });

        modelBuilder.Entity<DetkiProdUserRole>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name).IsRequired();

            entity.HasMany(x => x.Users).WithOne(y => y.Role).HasForeignKey(d => d.RoleId).IsRequired();

            var userRoles = Enum.GetValues<UserRole>().ToList();
            var userRoleEntities = userRoles.Select(x => DetkiProdUserRole.Create(x)).ToArray();

            entity.HasData(userRoleEntities);
        });

        modelBuilder.Entity<DetkiProdProject>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name).IsRequired();
            entity.Property(x => x.Year).IsRequired();
            entity.Property(x => x.Tools).IsRequired();
            entity.Property(x => x.VideoUrl).IsRequired();
        });
    }
}
