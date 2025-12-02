using DetkiProd.Domain.Enums;

namespace DetkiProd.Application.Interfaces;

public interface IJwtProvider
{
    string Generate(Guid userId, string userLogin, UserRole userRole);
}
