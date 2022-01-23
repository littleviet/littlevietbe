using Azure.Storage.Blobs;

namespace LittleViet.Infrastructure.Azure.AzureBlobStorage.Interface;

public interface IBaseBlobService
{
    Task<BlobContainerClient> GetBlobContainer(string connectionString, string containerName);
    Task<bool> IsExist(BlobContainerClient blobContainerClient, string blobPath);
    Task UploadFileToBlobAsync(BlobContainerClient blobContainerClient, string blobName, Stream filePath);
}

