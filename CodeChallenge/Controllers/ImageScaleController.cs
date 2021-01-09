using CodeChallenge.Model;
using CodeChallenge.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;

namespace CodeChallenge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageScaleController : ControllerBase
    {

        private readonly ILogger<ImageScaleController> _logger;
        private readonly IImageRepository _imageRepository;

        public ImageScaleController(
            ILogger<ImageScaleController> logger,
            IImageRepository imageRepository
            )
        {
            _logger = logger;
            _imageRepository = imageRepository;
        }

        [HttpGet("{imageName}")]
        //public FileStreamResult Get(ImageDetails imageDetails)
        public FileStreamResult Get(string imageName, string type, int width, int height, string watermark, string backgroundColour)
        {
            var stream = _imageRepository.GetSourceImageStream("01_04_2019_001103.png");

            var imageDetails = new ImageDetails()
            {
                ImageName = imageName,
                Type = type,
                Width = width,
                Height = height
            };

            return new FileStreamResult(stream, $"image/png")
            {
                // todo: will expect the name to not include the file extension
                FileDownloadName = $"{imageDetails.ImageName}_scaled_{imageDetails.Width}_{imageDetails.Height}.{imageDetails.Type}"
            };
        }

    }
}
