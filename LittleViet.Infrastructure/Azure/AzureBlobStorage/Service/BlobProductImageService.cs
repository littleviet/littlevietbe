using LittleViet.Infrastructure.Azure.AzureBlobStorage.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace LittleViet.Infrastructure.Azure.AzureBlobStorage.Service;

public class BlobProductImageService : BaseBlobService, IBlobProductImageService
{
    private string _connectionString;
    private readonly IConfiguration _configuration;
    public BlobProductImageService(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _connectionString = _configuration["ConnectionStrings:LittleVietContainer"];
    }

    public async Task<List<string>> CreateProductImages(List<IFormFile> productImages)
    {

        var blobContainer = await GetBlobContainer(_connectionString, "products");
        var imageLinks = new List<string>();

        foreach (var image in productImages)
        {
            if (image.Length > 0)
            {
                string file_Extension = Path.GetExtension(image.FileName);
                string filename = Guid.NewGuid() + "" + (!string.IsNullOrEmpty(file_Extension) ? file_Extension : ".jpg");

                await UploadFileToBlobAsync(blobContainer, filename, image.OpenReadStream());

                imageLinks.Add(new Uri(blobContainer.Uri.AbsoluteUri) + "/" + filename);
            }
        }

        return imageLinks;
    }
}

