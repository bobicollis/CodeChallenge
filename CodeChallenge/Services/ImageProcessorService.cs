using CodeChallenge.Model;
using Microsoft.Extensions.Logging;
using System.IO;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using CodeChallenge.Utils;

namespace CodeChallenge.Services
{
    public class ImageProcessorService : IImageProcessorService
    {
        private readonly Font _font;
        private readonly ILogger<ImageProcessorService> _logger;

        public ImageProcessorService(ILogger<ImageProcessorService> logger)
        {
            _font = SystemFonts.CreateFont("Arial", 10);
            _logger = logger;
        }

        public Stream ProcessImage(Stream sourceStream, ImageDetails details)
        {
            // todo: requirements don't rule out enlarging an image!
            IImageEncoder encoder = GetEncoder(details.Type);

            string backgroundColour = ColourParser.Parse(details.BackgroundColour);

            using (Image<Rgba32> outImage = new Image<Rgba32>(new Configuration(), details.Width, details.Height, Rgba32.ParseHex(backgroundColour)))
            using (Image sourceImage = Image.Load(sourceStream))
            {
                sourceImage.Mutate(ctx => ctx.Resize(new ResizeOptions()
                {
                    Size = new Size(details.Width, details.Height),
                    Mode = ResizeMode.Max,
                }));

                outImage.Mutate(ctx => ctx.DrawImage(sourceImage, CentredTopLeft(details.Width, details.Height, sourceImage.Width, sourceImage.Height), 1f));

                if (!string.IsNullOrEmpty(details.Watermark))
                {
                    outImage.Mutate(ctx => ctx.ApplyScalingWaterMarkSimple(_font, details.Watermark, Color.Pink, 5));
                }

                Stream outStream = new MemoryStream();
                outImage.Save(outStream, encoder);

                return outStream;
            };
        }

        private IImageEncoder GetEncoder(ImageType type)
        {
            switch (type)
            {
                case ImageType.Jpg:
                case ImageType.Jpeg:
                    return new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder();
                case ImageType.Bmp:
                    return new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder();
                case ImageType.Gif:
                    return new SixLabors.ImageSharp.Formats.Gif.GifEncoder();
                case ImageType.Tga:
                    return new SixLabors.ImageSharp.Formats.Tga.TgaEncoder();
                default: // All source images are png, so default to returning that
                    return new SixLabors.ImageSharp.Formats.Png.PngEncoder();
            }
        }

        private Point CentredTopLeft(int boundsWidth, int boundsHeight, int innerWidth, int innerHeght)
            => new Point((boundsWidth - innerWidth) / 2, (boundsHeight - innerHeght) / 2);

    }
}
