using LittleViet.Infrastructure.DateTime;
using Microsoft.Extensions.DependencyInjection;

namespace LittleViet.Infrastructure.Caching;

public static class StartupCacheExtensions 
{
    public static IServiceCollection AddAppInMemoryCacheServices(this IServiceCollection serviceCollection) =>
        serviceCollection
            .AddMemoryCache()
            .AddSingleton<ICache, MemoryCache>();
}