using Microsoft.AspNetCore.Http;

namespace LittleViet.Infrastructure.Azure.AzureBlobStorage.Interface;

public interface IBlobMenuService
{
    Task<string> UpdateMenuAsync(IFormFile menu);
}