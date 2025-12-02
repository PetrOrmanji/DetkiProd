using DetkiProd.Application.DTOs;

namespace DetkiProd.Application.Interfaces;

public interface IUserService
{
    Task<List<DetkiProdUserDto>> GetUsersAsync();
    Task DeleteUserAsync(Guid userId);
}