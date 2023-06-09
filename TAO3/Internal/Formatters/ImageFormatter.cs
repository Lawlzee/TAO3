using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Formatting;
using Microsoft.DotNet.Interactive;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using static Microsoft.DotNet.Interactive.Formatting.PocketViewTags;

namespace TAO3.Internal.Formatters;
internal static class ImageFormatter
{
    public static void Register()
    {
        Formatter.Register<Image>((image, writer) =>
        {
            writer.Write(CreatePocketView(image));
        }, HtmlFormatter.MimeType);

        Formatter.Register<Bitmap>((image, writer) =>
        {
            writer.Write(CreatePocketView(image));
        }, HtmlFormatter.MimeType);
    }

    private static PocketView CreatePocketView(Image image)
    {
        using MemoryStream stream = new MemoryStream();
        image.Save(stream, ImageFormat.Png);
        stream.Flush();
        byte[] data = stream.ToArray();
        return CreateImgageTag(data, image.Height, image.Width);
    }

    private static PocketView CreateImgageTag(byte[] data, int height, int width)
    {
        var imageSource = $"data:image/png;base64, {Convert.ToBase64String(data)}";
        PocketView imgTag = img[src: imageSource, height: height, width: width]();
        return imgTag;
    }
}
