using CodeChallenge.Model;
using System.IO;

namespace CodeChallenge.Repository
{
    public interface IImageRepository
    {
        void AddCacheDetails(string chacheFilename, ImageDetails details);
        Stream GetCacheImageStream(ImageDetails details);
        string GetNewCacheFilenameForDetails(ImageDetails details);
        Stream GetSourceImageStream(string imageName);
        bool IsInCache(ImageDetails details);
    }
}