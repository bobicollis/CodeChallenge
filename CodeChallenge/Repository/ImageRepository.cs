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
    public class ImageRepository : IImageRepository
    {

        private readonly string _imageSourcePath;
        private readonly string _imageCachePath;
        private readonly Dictionary<string, string> _cacheNames;
        private readonly ILogger<ImageRepository> _logger;

        public ImageRepository(ILogger<ImageRepository> logger)
        {
            // Would be from appsettings for base path and source / cache, but har coded for now.
            string appBasePath = @"D:\Code\Atom";
            _imageSourcePath = Path.Combine(appBasePath, "product_images");
            _imageCachePath = Path.Combine(appBasePath, "cache_images");
            _cacheNames = new Dictionary<string, string>();
            _logger = logger;
        }

        public Stream GetSourceImageStream(string imageName)
        {
            try
            {
                string filename = Path.Combine(_imageSourcePath, imageName);
                return File.OpenRead(filename);
            }
            catch (FileNotFoundException ex)
            {
                throw new SourceImageNotFoundException(imageName, ex);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Stream GetCacheImageStream(ImageDetails details)
        {
            string cacheName = ImageCacheName(details);
            try
            {
                string filename = Path.Combine(_imageCachePath, cacheName);
                return File.OpenRead(filename);
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError(ex, "Cache image not found", details);
                throw new CacheImageNotFoundException(ex);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public async Task AddCacheImageAsync(FileStream image, ImageDetails details)
        //{
        //    try
        //    {
        //        Guid name = new Guid();
        //        string filename = Path.Combine(_imageCachePath, $"{name}.{details.Type}");

        //        using (var fs = File.Create(filename))
        //        {
        //            await image.CopyToAsync(fs);
        //            await image.FlushAsync();
        //        }

        //        // Now that the image is saved, add the description to the names cache.
        //        _cacheNames.Add(new ImageCacheKey(details), name);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error storing cache image", details);
        //        throw;
        //    }
        //}

        public string GetNewCacheFilenameForDetails(ImageDetails details)
        {
            Guid name = Guid.NewGuid();
            return Path.Combine(_imageCachePath, $"{name}.{details.Type.ToString().ToLower()}");
        }

        public void AddCacheDetails(string cacheFilename, ImageDetails details)
        {
            _logger.LogInformation($"Creating cache entry for {cacheFilename}", details);
            _cacheNames.Add(GetKey(details), cacheFilename);
        }

        public bool IsInCache(ImageDetails details)
            => _cacheNames.ContainsKey(GetKey(details));

        private string ImageCacheName(ImageDetails details)
            => _cacheNames[GetKey(details)];

        /// <summary>
        /// Hack to allow lookup of images by details without implementing a database. 
        /// If we used details for filenames then crazy long watermark text could exceed max filename size.
        /// </summary>
        private string GetKey(ImageDetails details)
            => $"{details.ImageName}^{details.Width}^{details.Height}^{details.Type}^{details.BackgroundColour}^{details.Watermark}";
    }
}
