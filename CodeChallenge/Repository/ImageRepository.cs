using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using CodeChallenge.Model;
using System.Diagnostics.CodeAnalysis;

namespace CodeChallenge.Repository
{
    public class ImageRepository
    {

        private readonly string _imageSourcePath;
        private readonly string _imageCachePath;
        private readonly Dictionary<ImageCacheKey, Guid> _chacheNames;

        public ImageRepository()
        {
            // Would be from appsettings for base path and source / cache (in Azure blob storage)
            string appBasePath = @"D:\Code\Atom";
            _imageSourcePath = Path.Combine(appBasePath, "product_images");
            _imageCachePath = Path.Combine(appBasePath, "cache_images");
            _chacheNames = new Dictionary<ImageCacheKey, Guid>();
        }

        public Stream GetSourceImageStream(string imageName)
        {
            string filename = Path.Combine(_imageCachePath, imageName);
            return File.OpenRead(filename);
        }

        public Stream GetCacheImageStream(ImageDetails details)
        {
            Guid cacheName = ImageCacheName(details);
            string filename =  Path.Combine(_imageCachePath, $"{cacheName}.{details.Type}");
            return File.OpenRead(filename);
        }

        public void AddCacheImage(Stream image, ImageDetails details)
        {

        }

        public bool IsInCache(ImageDetails details)
        {
            var key = new ImageCacheKey(details);
            return _chacheNames.ContainsKey(key);
        }

        private Guid ImageCacheName(ImageDetails details)
        {
            var key = new ImageCacheKey(details);
            return _chacheNames[key];
        }

        /// <summary>
        /// Hack to allow lookup of images by details without implementing a database. 
        /// If we used details for filenames then crazy long watermark text could exceed max filename size.
        /// </summary>
        private class ImageCacheKey : IEquatable<ImageCacheKey>
        {
            public ImageDetails ImageDetails { get; }

            public ImageCacheKey(ImageDetails imageDetails)
            {
                ImageDetails = imageDetails;
            }

            public bool Equals(ImageCacheKey other)
            {
                return this.ImageDetails.Name == other.ImageDetails.Name
                    && this.ImageDetails.Type == other.ImageDetails.Type
                    && this.ImageDetails.Width == other.ImageDetails.Width
                    && this.ImageDetails.Height == other.ImageDetails.Height
                    && this.ImageDetails.Watermark == other.ImageDetails.Watermark
                    && this.ImageDetails.BackgroundColour == other.ImageDetails.BackgroundColour;
            }
        }
    }
}
