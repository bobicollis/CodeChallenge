using CodeChallenge.Model;
using System.IO;
using System.Threading.Tasks;

namespace CodeChallenge.Repository
{
    public interface IImageRepository
    {
        Task AddCacheImageAsync(Stream image, ImageDetails details);
        Stream GetCacheImageStream(ImageDetails details);
        Stream GetSourceImageStream(string imageName);
        bool IsInCache(ImageDetails details);
    }
}