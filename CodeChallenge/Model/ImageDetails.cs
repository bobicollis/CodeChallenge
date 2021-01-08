using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Model
{
    public class ImageDetails
    {
        [Required]
        public string Name { get; set; }
        public string Type { get; set; }
        [Required]
        public int Width { get; set; }
        [Required]
        public int Height { get; set; }
        public string Watermark { get; set; }
        public string BackgroundColour { get; set; }
    }
}
