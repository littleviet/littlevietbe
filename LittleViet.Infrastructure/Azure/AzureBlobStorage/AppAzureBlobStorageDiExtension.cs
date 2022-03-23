using LittleViet.Infrastructure.Azure.AzureBlobStorage.Interface;
using LittleViet.Infrastructure.Azure.AzureBlobStorage.Service;
using Microsoft.Extensions.DependencyInjection;

namespace LittleViet.Infrastructure.Azure.AzureBlobStorage;

public static class AppAzureBlobStorageDiExtensions
{
    public static IServiceCollection AddAppAzureBlobStorage(this IServiceCollection services) =>
        services.AddScoped<IBlobProductImageService, BlobProductImageService>();
}