using System;

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
