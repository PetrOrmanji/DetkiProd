namespace DetkiProd.Domain.Exceptions;

public class ProjectNotFoundException : DomainException
{
    public ProjectNotFoundException(Guid id) : base($"Project with id=[{id}] not found.")
    {
    }
}
