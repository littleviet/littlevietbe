using Azure.Storage.Blobs;
using LittleViet.Infrastructure.Azure.AzureBlobStorage.Interface;

namespace LittleViet.Infrastructure.Azure.AzureBlobStorage.Service;

public class BaseBlobService : IBaseBlobService
{
    public BaseBlobService()
    {
    }

    public async Task<BlobContainerClient> GetBlobContainer(string connectionString, string containerName)
    {
        BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

        var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

        if(blobContainerClient == null)
        {
            blobContainerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);
        }

        return blobContainerClient;
    }

    public async Task<bool> IsExist(BlobContainerClient blobContainerClient, string blobPath)
    {
        BlobClient blobClient = blobContainerClient.GetBlobClient(blobPath);

        if (!await blobClient.ExistsAsync())
        {
            return false;
        }

        return true;
    }

    public async Task UploadFileToBlobAsync(BlobContainerClient blobContainerClient, string blobName, Stream filePath)
    {
        var actualBlob = blobContainerClient.GetBlobClient(blobName);

        await actualBlob.UploadAsync(filePath);
    }
}

