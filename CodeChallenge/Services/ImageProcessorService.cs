using CodeChallenge.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace CodeChallenge.Services
{
    public class ImageProcessorService
    {
        private readonly Font _font;
        private readonly ILogger<ImageProcessorService> _logger;

        public ImageProcessorService(ILogger<ImageProcessorService> logger)
        {
            _font = SystemFonts.CreateFont("Arial", 10);
            _logger = logger;
        }

        public Stream ProcessImage(Stream sourceImage, ImageDetails details)
        {
            IImageEncoder encoder = GetEncoder(details.Type);

            using (Image image = Image.Load(sourceImage))
            {
                image.Mutate(ctx => ctx.Resize(new ResizeOptions()
                {
                    Size = new Size(details.Width, details.Height),
                    Mode = ResizeMode.Max,                   
                }));

                if (!string.IsNullOrEmpty(details.Watermark))
                {
                    image.Mutate(ctx => ctx.ApplyScalingWaterMarkSimple(_font, details.Watermark, Color.Pink, 5));
                }

                var newStream = new MemoryStream();
                image.Save(newStream, encoder);
                return newStream;
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

    }
}
