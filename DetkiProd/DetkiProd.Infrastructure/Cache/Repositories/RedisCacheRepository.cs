using DetkiProd.Application.Interfaces;
using StackExchange.Redis;

namespace DetkiProd.Infrastructure.Cache.Repositories;

public class RedisCacheRepository : ICacheRepository
{
    private readonly IDatabase _cacheDb;

    public RedisCacheRepository(IConnectionMultiplexer mux)
    {
        _cacheDb = mux.GetDatabase();
    }

    public async Task SetAsync(string key, string value, TimeSpan ttl)
    {
        await _cacheDb.StringSetAsync(key, value, ttl);
    }

    public async Task<string?> GetAsync(string key)
    {
        return await _cacheDb.StringGetAsync(key);
    }

    public async Task RemoveAsync(string key)
    {
        await _cacheDb.KeyDeleteAsync(key);
    }
}
