﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Repository
{
    public class SourceImageNotFoundException : Exception
    {
        public string SourceImageFilename { get; }
        public SourceImageNotFoundException(string filename, Exception inner)
            : base($"Source image '{filename}' not found.", inner)
        {
            SourceImageFilename = filename;
        }
    }
}
