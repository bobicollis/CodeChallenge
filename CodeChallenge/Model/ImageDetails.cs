using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Model
{
    [BindProperties(SupportsGet = true)]
    public class ImageDetails
    {
        [Required]
        public string ImageName { get; set; }
        public ImageType Type { get; set; }
        [Required]
        public int Width { get; set; }
        [Required]
        public int Height { get; set; }
        public string Watermark { get; set; }
        public string BackgroundColour { get; set; }

        public ImageDetails() { }
    }
}
