using LittleViet.Infrastructure.Email.Interface;
using LittleViet.Infrastructure.Email.Service;
using Microsoft.Extensions.DependencyInjection;

namespace LittleViet.Infrastructure.Email;

public static class StartupEmailConfigurationExtensions
{
    public static IServiceCollection AddAppEmailServices(this IServiceCollection serviceCollection) => serviceCollection
        .AddScoped<IEmailService, EmailService>()
        .AddScoped<ITemplateService, TemplateService>();
}