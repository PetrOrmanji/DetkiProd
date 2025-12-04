using DetkiProd.Domain.Enums;

namespace DetkiProd.Application.Interfaces;

public interface ICacheService
{
    Task<TelegramUserState> GetTelegramUserStateAsync(long chatId);
    Task SetTelegramUserStateAsync(long chatId, TelegramUserState userState);
}
