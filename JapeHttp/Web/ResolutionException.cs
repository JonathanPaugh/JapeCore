using System;

namespace JapeHttp
{
    public class ResolutionException : Exception
    {
        public ResolutionException() {}
        public ResolutionException(string message) : base(message) {}
    }
}