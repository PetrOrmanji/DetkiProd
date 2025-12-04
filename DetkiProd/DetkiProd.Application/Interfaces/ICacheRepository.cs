namespace DetkiProd.Application.Interfaces;

public interface ICacheRepository
{
    Task SetAsync(string key, string value, TimeSpan ttl);
    Task<string?> GetAsync(string key);
    Task RemoveAsync(string key);
}
