using DetkiProd.Application.DTOs;

namespace DetkiProd.Application.Interfaces;

public interface IAuthService
{
    Task<string> LoginAsync(string login, string password);
    Task<string> RegisterAsAdminAsync(string login, string password);
    Task<DetkiProdUserDto> RegisterTelegramUserAsAdminAsync(long telegramUserId);
}