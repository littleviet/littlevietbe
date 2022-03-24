using Microsoft.AspNetCore.StaticFiles;

namespace LittleViet.Infrastructure.Utilities;

public static class MimeHelper
{
    public static string GetMimeType(string fileName, FileExtensionContentTypeProvider contentTypeProvider = default)
    {
        contentTypeProvider ??= new FileExtensionContentTypeProvider();
        return contentTypeProvider.TryGetContentType(fileName, out var contentType)
            ? contentType
            : "application/octet-stream";
    }
}