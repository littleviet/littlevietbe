using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using LittleViet.Infrastructure.Azure.AzureBlobStorage.Interface;
using LittleViet.Infrastructure.Utilities;

namespace LittleViet.Infrastructure.Azure.AzureBlobStorage.Service;

internal class BaseBlobService : IBaseBlobService
{
    public async Task<BlobContainerClient> GetBlobContainer(string connectionString, string containerName)
    {
        var blobServiceClient = new BlobServiceClient(connectionString);

        var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName) ??
                                  await blobServiceClient.CreateBlobContainerAsync(containerName);

        return blobContainerClient;
    }

    public async Task<bool> IsExist(BlobContainerClient blobContainerClient, string blobPath)
    {
        var blobClient = blobContainerClient.GetBlobClient(blobPath);

        return (await blobClient.ExistsAsync()).Value;
    }

    public async Task UploadFileToBlobAsync(BlobContainerClient blobContainerClient, string blobName, Stream fileStream)
    {
        var actualBlob = blobContainerClient.GetBlobClient(blobName);
        await actualBlob.UploadAsync(fileStream, new BlobHttpHeaders()
        {
            ContentType = MimeHelper.GetMimeType(blobName)
        });
    }
}