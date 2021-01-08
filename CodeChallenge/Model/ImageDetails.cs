using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Model
{
    public class ImageDetails
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Watermark { get; set; }
        public string BackgroundColour { get; set; }
    }
}
