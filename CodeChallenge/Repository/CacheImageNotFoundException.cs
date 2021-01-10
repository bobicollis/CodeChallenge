using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Repository
{
    public class CacheImageNotFoundException : Exception
    {
        public CacheImageNotFoundException(Exception inner)
            : base($"Cache image not found.", inner)
        {
        }
    }
}
