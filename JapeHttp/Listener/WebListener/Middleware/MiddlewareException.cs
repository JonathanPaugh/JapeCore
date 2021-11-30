using System;

namespace JapeHttp
{
    public class MiddlewareException : Exception
    {
        public MiddlewareException() {}
        public MiddlewareException(string message) : base(message) {}
    }
}
