using LittleViet.Infrastructure.Azure.AzureBlobStorage.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace LittleViet.Infrastructure.Azure.AzureBlobStorage.Service;

internal class BlobProductImageService : BaseBlobService, IBlobProductImageService
{
    private readonly AzureSettings _azureSettings;
    private readonly string _connectionString;
    public BlobProductImageService(IConfiguration configuration, IOptions<AzureSettings> azureSettings)
    {
        _azureSettings = azureSettings.Value;
        _connectionString = configuration?.GetConnectionString("LittleVietContainer") ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<List<string>> CreateProductImages(List<IFormFile> productImages)
    {
        var blobContainer = await GetBlobContainer(_connectionString, _azureSettings.BlobStorageSettings.ProductImageContainer);
        var imageLinks = new List<string>();

        foreach (var image in productImages)
        {
            if (image.Length > 0)
            {
                var file_Extension = Path.GetExtension(image.FileName);
                var filename = Guid.NewGuid() + (!string.IsNullOrEmpty(file_Extension) ? file_Extension : ".jpg");

                await UploadFileToBlobAsync(blobContainer, filename, image.OpenReadStream());

                imageLinks.Add(new Uri(blobContainer.Uri.AbsoluteUri) + "/" + filename);
            }
        }

        return imageLinks;
    }
}

