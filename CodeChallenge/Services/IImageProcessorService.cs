using CodeChallenge.Model;
using System.IO;
using System.Threading.Tasks;

namespace CodeChallenge.Services
{
    public interface IImageProcessorService
    {
        Task<Stream> ProcessImageAsync(Stream sourceStream, ImageDetails details);
    }
}