using CodeChallenge.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Utils
{
    public static class FilenameResponse
    {
        public static string FilenameForImageDetails(ImageDetails details) =>
            $"{details.ImageName}_scaled_{details.Width}_{details.Height}.{details.Type}";
    }
}
