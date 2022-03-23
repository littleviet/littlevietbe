namespace LittleViet.Infrastructure.Azure;

public class AzureSettings
{
    public const string ConfigSection = "Azure";
    public string AppInsightsKey { get; set; }
    public AzureBlobStorageSettings BlobStorageSettings { get; set; } = new();
}

public class AzureBlobStorageSettings
{
    public string ProductImageContainer { get; set; } = "products";
}