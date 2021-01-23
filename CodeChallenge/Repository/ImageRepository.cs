using System;
using Microsoft.Extensions.Logging;
using System.IO;
using CodeChallenge.Model;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace CodeChallenge.Repository
{
    /// <summary>
    /// The requirements are for something scalable, so create a repository with an interface that 
    /// would make sense for Azure blob storage. 
    /// There could be an argument for caching intermediate versions of images without the watermark, for example. 
    /// In reality, we would look at costs / benefits before spending time implementing it.
    /// The data for image cache details is not persisted, so will be lost when restarting.
    /// </summary>
    public class ImageRepository : IImageRepository
    {

        private readonly string _imageSourcePath;
        private readonly string _imageCachePath;
        private readonly ConcurrentDictionary<string, string> _cacheNames;
        private readonly ILogger<ImageRepository> _logger;

        public ImageRepository(IOptions<RepositoryLocationOptions> options, ILogger<ImageRepository> logger)
        {
            _imageSourcePath = options.Value.SourceImages;
            _imageCachePath = options.Value.CacheImages;
            _cacheNames = new ConcurrentDictionary<string, string>();
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
            string cacheName = GetImageCacheFilename(details);
            try
            {
                string filename = Path.Combine(_imageCachePath, cacheName);
                _logger.LogDebug($"Using cached image for {details.ImageName}", details);
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

        public async Task AddCacheImageAsync(Stream image, ImageDetails details)
        {
            string name = GetCacheFilenameForDetails(details);
            string filename = Path.Combine(_imageCachePath, $"{name}.{details.Type}");
            try
            {
                using (var fs = File.Create(filename))
                {
                    long location = image.Seek(0, SeekOrigin.Begin);
                    await image.CopyToAsync(fs);
                    await image.FlushAsync();
                }

                // Now that the file has been written successfully, add to the cache names lookup
                if (!_cacheNames.TryAdd(GetKey(details), filename))
                {
                    _logger.LogWarning($"Failed to add {filename} to cache details, file will be inaccessible.", details);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving image to cache for filename {filename}. File if present may be corrupt.", details);
                throw ex;
            }
        }

        public bool IsInCache(ImageDetails details)
            => _cacheNames.ContainsKey(GetKey(details));

        private string GetCacheFilenameForDetails(ImageDetails details)
            => Path.Combine(_imageCachePath, $"{Guid.NewGuid()}.{details.Type.ToString().ToLower()}");

        private string GetImageCacheFilename(ImageDetails details)
            => _cacheNames[GetKey(details)];

        private string GetKey(ImageDetails details)
            => $"{details.ImageName}^{details.Width}^{details.Height}^{details.Type}^{details.BackgroundColour}^{details.Watermark}";
    }
}
