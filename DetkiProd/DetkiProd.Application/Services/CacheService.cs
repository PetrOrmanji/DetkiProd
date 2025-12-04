using DetkiProd.Application.Interfaces;
using DetkiProd.Domain.Enums;

namespace DetkiProd.Application.Services;

public class CacheService : ICacheService
{
    private const string DetkiProdTag = "DetkiProd";

    private readonly ICacheRepository _cacheRepository;

    public CacheService(ICacheRepository cacheRepository)
    {
        _cacheRepository = cacheRepository ?? throw new ArgumentException("Cache Repository cannot be null", nameof(cacheRepository));
    }

    public async Task<TelegramUserState> GetTelegramUserStateAsync(long chatId)
    {
        var tgUserStateString = await _cacheRepository.GetAsync(GetKey(chatId));

        return Enum.TryParse<TelegramUserState>(tgUserStateString, out var tgUserState) ? tgUserState : TelegramUserState.None;
    }

    public async Task SetTelegramUserStateAsync(long chatId, TelegramUserState userState)
    {
        var value = userState.ToString();

        var ttl = TimeSpan.FromDays(1);

        await _cacheRepository.SetAsync(GetKey(chatId), value, ttl);
    }

    private string GetKey(long chatId) => $"{DetkiProdTag}_{chatId}";
}
