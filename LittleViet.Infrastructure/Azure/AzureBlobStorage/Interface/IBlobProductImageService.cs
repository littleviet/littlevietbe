using Microsoft.AspNetCore.Http;

namespace LittleViet.Infrastructure.Azure.AzureBlobStorage.Interface;

public interface IBlobProductImageService : IBaseBlobService
{
    Task<List<string>> CreateProductImages(List<IFormFile> productImages);
}

