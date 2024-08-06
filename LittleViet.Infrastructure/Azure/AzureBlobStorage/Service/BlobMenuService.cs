using LittleViet.Infrastructure.Azure.AzureBlobStorage.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace LittleViet.Infrastructure.Azure.AzureBlobStorage.Service;

internal class BlobMenuService : BaseBlobService, IBlobMenuService
{
    private const string MenuFileName = "menu.pdf";
    private readonly AzureSettings _azureSettings;
    private readonly string _connectionString;

    public BlobMenuService(IConfiguration configuration, IOptions<AzureSettings> azureSettings)
    {
        _azureSettings = azureSettings.Value;
        _connectionString = configuration?.GetConnectionString("LittleVietContainer") ??
                            throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<string> UpdateMenuAsync(IFormFile menu)
    {
        var blobContainer =
            await GetBlobContainer(_connectionString, _azureSettings.BlobStorageSettings.ProductImageContainer);

        await UploadFileToBlobAsync(blobContainer, MenuFileName, menu.OpenReadStream());

        return $"{new Uri(blobContainer.Uri.AbsoluteUri)}/{MenuFileName}";
    }
}