using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using CodeChallenge.Model;
using System.Diagnostics.CodeAnalysis;

namespace CodeChallenge.Repository
{
    /// <summary>
    /// The requirements are for something scalable, so create a repository with an interface that 
    /// would make sense for Azure blob storage. 
    /// There could be an argument for caching intermediate versions of images without the watermark, for example. 
    /// In reality, we would look at costs / benefits before spending time implementing it.
    /// </summary>
    public class ImageRepository
    {

        private readonly string _imageSourcePath;
        private readonly string _imageCachePath;
        private readonly Dictionary<ImageCacheKey, Guid> _cacheNames;
        private readonly ILogger<ImageRepository> _logger;

        public ImageRepository(ILogger<ImageRepository> logger)
        {
            // Would be from appsettings for base path and source / cache, but har coded for now.
            string appBasePath = @"D:\Code\Atom";
            _imageSourcePath = Path.Combine(appBasePath, "product_images");
            _imageCachePath = Path.Combine(appBasePath, "cache_images");
            _cacheNames = new Dictionary<ImageCacheKey, Guid>();
            _logger = logger;
        }

        public Stream GetSourceImageStream(string imageName)
        {
            string filename = Path.Combine(_imageSourcePath, imageName);
            return File.OpenRead(filename);
        }

        public Stream GetCacheImageStream(ImageDetails details)
        {
            Guid cacheName = ImageCacheName(details);
            string filename =  Path.Combine(_imageCachePath, $"{cacheName}.{details.Type}");
            return File.OpenRead(filename);
        }

        public async Task AddCacheImageAsync(Stream image, ImageDetails details)
        {
            try
            {
                Guid name = new Guid();
                string filename = Path.Combine(_imageCachePath, $"{name}.{details.Type}");

                using (var fs = File.Create(filename))
                {
                    await image.CopyToAsync(fs);
                    await image.FlushAsync();
                }

                // Now that the image is saved, add the description to the names cache.
                _cacheNames.Add(new ImageCacheKey(details), name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing cache image", details);
                throw;
            }
        }

        public bool IsInCache(ImageDetails details)
        {
            var key = new ImageCacheKey(details);
            return _cacheNames.ContainsKey(key);
        }

        private Guid ImageCacheName(ImageDetails details)
        {
            var key = new ImageCacheKey(details);
            return _cacheNames[key];
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
