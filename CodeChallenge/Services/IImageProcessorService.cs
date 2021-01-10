using CodeChallenge.Model;
using System.IO;

namespace CodeChallenge.Services
{
    public interface IImageProcessorService
    {
        Stream ProcessImageAsync(Stream sourceStream, ImageDetails details, string cacheFilename);
    }
}