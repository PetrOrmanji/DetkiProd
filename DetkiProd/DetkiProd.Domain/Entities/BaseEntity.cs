namespace DetkiProd.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; }
    public DateTime CreatedAt { get; }

    public BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.Now;
    }
}
