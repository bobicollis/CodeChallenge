using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CodeChallenge.Model;
using CodeChallenge.Repository;

namespace CodeChallenge.Tests.Repository
{
    class ImageRepositoryFake : IImageRepository
    {
        public List<string> ValidFilePaths = new List<string>();

        public ImageRepositoryFake()
        {

        }

        public Task AddCacheImageAsync(Stream image, ImageDetails details)
        {
            throw new NotImplementedException();
        }

        public Stream GetCacheImageStream(ImageDetails details)
        {
            throw new NotImplementedException();
        }

        public Stream GetSourceImageStream(string imageName)
        {
            throw new NotImplementedException();
        }

        public bool IsInCache(ImageDetails details)
        {
            throw new NotImplementedException();
        }
    }
}
