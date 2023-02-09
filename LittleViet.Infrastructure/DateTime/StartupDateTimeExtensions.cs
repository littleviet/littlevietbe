using LittleViet.Infrastructure.Email.Interface;
using LittleViet.Infrastructure.Email.Service;
using Microsoft.Extensions.DependencyInjection;

namespace LittleViet.Infrastructure.DateTime;

public static class StartupDateTimeConfigurationExtensions
{
    public static IServiceCollection AddAppDateTimeServices(this IServiceCollection serviceCollection) =>
        serviceCollection
            .AddSingleton<IDateTimeService, DateTimeService>();
}