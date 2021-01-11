using CodeChallenge.Model;
using System.IO;

namespace CodeChallenge.Services
{
    public interface IImageProcessorService
    {
        //Stream ProcessImage(Stream sourceStream, ImageDetails details, string cacheFilename);
        Stream ProcessImage(Stream sourceStream, ImageDetails details);
    }
}