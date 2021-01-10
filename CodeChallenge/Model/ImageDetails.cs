using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace CodeChallenge.Model
{
    [BindProperties(SupportsGet = true)]
    public class ImageDetails
    {
        [Required]
        public string ImageName { get; set; }
        public ImageType Type { get; set; }
        [Range(10, 10000)]
        public int Width { get; set; }
        [Range(10, 10000)]
        public int Height { get; set; }
        public string Watermark { get; set; }
        [RegularExpression(@"^#?(([0-9a-fA-F]{2}){3,4})$", ErrorMessage = "BackgroundColour must be a hex value in RGB or RGBA format.")]
        public string BackgroundColour { get; set; }

        //public ImageDetails() { }
    }
}
