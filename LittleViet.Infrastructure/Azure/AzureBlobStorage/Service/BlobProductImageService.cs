using LittleViet.Infrastructure.Azure.AzureBlobStorage.Interface;
using LittleViet.Infrastructure.Images;
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
                await using var imageStream = image.OpenReadStream();
                await using var webpImageSream = await WebpImageConverter.ConvertToWebp(imageStream);
                var filename = Guid.NewGuid() + ".webp";

                await UploadFileToBlobAsync(blobContainer, filename, webpImageSream);

                imageLinks.Add(new Uri(blobContainer.Uri.AbsoluteUri) + "/" + filename);
            }
        }

        return imageLinks;
    }
}

