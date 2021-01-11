﻿using CodeChallenge.Model;
using CodeChallenge.Repository;
using CodeChallenge.Services;
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

        //[HttpGet("{imageName}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public IActionResult Get(string imageName, ImageType type, [FromQuery] int width, [FromQuery] int height, [FromQuery] string watermark, [FromQuery] string backgroundColour)
        //{
        //    var imageDetails = new ImageDetails()
        //    {
        //        ImageName = imageName,
        //        Type = type,
        //        Width = width,
        //        Height = height,
        //        Watermark = watermark,
        //        BackgroundColour = backgroundColour
        //    };

        //    if (!TryValidateModel(imageDetails, nameof(ImageDetails)))
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    try
        //    {
        //        Stream outStream;

        //        if (_imageRepository.IsInCache(imageDetails))
        //        {
        //            outStream = _imageRepository.GetCacheImageStream(imageDetails);
        //        }
        //        else
        //        {
        //            var stream = _imageRepository.GetSourceImageStream(imageDetails.ImageName);
        //            string cacheFilename = _imageRepository.GetNewCacheFilenameForDetails(imageDetails);
        //            outStream = _imageProcessorService.ProcessImage(stream, imageDetails, cacheFilename);
        //            _imageRepository.AddCacheDetails(cacheFilename, imageDetails);
        //        }

        //        string resultFilename = $"{imageDetails.ImageName}_scaled_{imageDetails.Width}_{imageDetails.Height}.{imageDetails.Type}";

        //        return new FileStreamResult(outStream, $"image/png") { FileDownloadName = resultFilename };
        //    }
        //    catch (SourceImageNotFoundException ex)
        //    {
        //        return NotFound(imageDetails);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Unexpected error producing image", imageDetails);
        //        return Problem("Internal error processing image request.",null, StatusCodes.Status500InternalServerError);
        //    }
        //}

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

            try
            {
                Stream outStream;

                if (_imageRepository.IsInCache(imageDetails))
                {
                    outStream = _imageRepository.GetCacheImageStream(imageDetails);
                }
                else
                {
                    var stream = _imageRepository.GetSourceImageStream(imageDetails.ImageName);
                    outStream = _imageProcessorService.ProcessImage(stream, imageDetails);
                    await _imageRepository.AddCacheImageAsync(outStream, imageDetails);
                    outStream.Seek(0, SeekOrigin.Begin);
                }

                string resultFilename = $"{imageDetails.ImageName}_scaled_{imageDetails.Width}_{imageDetails.Height}.{imageDetails.Type}";

                return new FileStreamResult(outStream, $"image/png") { FileDownloadName = resultFilename };
            }
            catch (SourceImageNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Source image not found: {imageName}", imageDetails);
                return NotFound(imageDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error producing image", imageDetails);
                return Problem("Internal error processing image request.", null, StatusCodes.Status500InternalServerError);
            }
        }

        public string FilenameForDetails(ImageDetails details) =>
            $"{details.ImageName}_scaled_{details.Width}_{details.Height}.{details.Type}";
    }
}
