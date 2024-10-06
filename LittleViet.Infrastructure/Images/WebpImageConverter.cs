using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;

namespace LittleViet.Infrastructure.Images;

public class WebpImageConverter 
{
    public static async Task<Stream> ConvertToWebp(Stream imageStream)
    {
        using var myImage = await Image.LoadAsync(imageStream, CancellationToken.None);

        var outStream = new MemoryStream();
            
        await myImage.SaveAsync(outStream, new WebpEncoder());

        return outStream;
    }
}