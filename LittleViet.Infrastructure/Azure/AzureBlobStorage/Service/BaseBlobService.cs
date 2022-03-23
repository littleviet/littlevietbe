using Azure.Storage.Blobs;
using LittleViet.Infrastructure.Azure.AzureBlobStorage.Interface;

namespace LittleViet.Infrastructure.Azure.AzureBlobStorage.Service;

internal class BaseBlobService : IBaseBlobService
{

    public async Task<BlobContainerClient> GetBlobContainer(string connectionString, string containerName)
    {
        var blobServiceClient = new BlobServiceClient(connectionString);

        var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName) ?? await blobServiceClient.CreateBlobContainerAsync(containerName);

        return blobContainerClient;
    }

    public async Task<bool> IsExist(BlobContainerClient blobContainerClient, string blobPath)
    {
        var blobClient = blobContainerClient.GetBlobClient(blobPath);

        return (await blobClient.ExistsAsync()).Value;
    }

    public async Task UploadFileToBlobAsync(BlobContainerClient blobContainerClient, string blobName, Stream filePath)
    {
        var actualBlob = blobContainerClient.GetBlobClient(blobName);

        await actualBlob.UploadAsync(filePath);
    }
}

