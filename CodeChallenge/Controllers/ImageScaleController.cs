using CodeChallenge.Model;
using CodeChallenge.Repository;
using CodeChallenge.Services;
using static CodeChallenge.Utils.FilenameResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CodeChallenge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageScaleController : ControllerBase
    {

        private readonly ILogger<ImageScaleController> _logger;
        private readonly IImageRepository _imageRepository;
        private readonly IImageProcessorService _imageProcessorService;

        public ImageScaleController(
            ILogger<ImageScaleController> logger,
            IImageRepository imageRepository,
            IImageProcessorService imageProcessorService
            )
        {
            _logger = logger;
            _imageRepository = imageRepository;
            _imageProcessorService = imageProcessorService;
        }

        [HttpGet("{imageName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(string imageName, ImageType type, [FromQuery] int width, [FromQuery] int height, [FromQuery] string watermark, [FromQuery] string backgroundColour)
        {
            var imageDetails = new ImageDetails()
            {
                ImageName = imageName,
                Type = type,
                Width = width,
                Height = height,
                Watermark = watermark,
                BackgroundColour = backgroundColour
            };

            if (!TryValidateModel(imageDetails, nameof(ImageDetails)))
            {
                return BadRequest(ModelState);
            }

            Stream outStream = null;

            try
            {
                if (_imageRepository.IsInCache(imageDetails))
                {
                    outStream = _imageRepository.GetCacheImageStream(imageDetails);
                }
                else
                {
                    using (var stream = _imageRepository.GetSourceImageStream(imageDetails.ImageName))
                    {
                        outStream = _imageProcessorService.ProcessImage(stream, imageDetails);
                        await _imageRepository.AddCacheImageAsync(outStream, imageDetails);
                        outStream.Seek(0, SeekOrigin.Begin);
                    }
                }

                string resultFilename = FilenameForImageDetails(imageDetails);

                return new FileStreamResult(outStream, $"image/{imageDetails.Type}") { FileDownloadName = resultFilename };
            }
            catch (SourceImageNotFoundException ex)
            {
                outStream?.DisposeAsync();
                _logger.LogWarning(ex, $"Source image not found: {imageName}", imageDetails);
                return NotFound(imageDetails);
            }
            catch (Exception ex)
            {
                outStream?.DisposeAsync();
                _logger.LogError(ex, "Unexpected error producing image", imageDetails);
                return Problem("Internal error processing image request.", null, StatusCodes.Status500InternalServerError);
            }
        }
    }
}
