using Microsoft.Extensions.Caching.Memory;

namespace LittleViet.Infrastructure.Caching;

public interface ICache
{
    Task SetAsync<T>(string key, T value, TimeSpan? timeSpan = null);
    Task<(bool Found, T Value)> TryGetAsync<T>(string key);
    Task InvalidateAsync(params string[] keys);
}

internal class MemoryCache : ICache
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCache(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? timeSpan = null)
    {
        _memoryCache.Set(key, value,
            new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = timeSpan ?? TimeSpan.FromDays(14) });
        return Task.CompletedTask;
    }

    public Task<(bool Found, T? Value)> TryGetAsync<T>(string key) =>
        Task.FromResult(_memoryCache.TryGetValue<T>(key, out var value) ? (true, value) : (false, default(T)));

    public Task InvalidateAsync(params string[] keys)
    {
        foreach (var key in keys)
        {
            _memoryCache.Remove(key);
        }

        return Task.CompletedTask;
    }
}