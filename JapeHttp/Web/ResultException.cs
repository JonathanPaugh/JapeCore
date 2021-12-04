using System;

namespace JapeHttp
{
    public class ResultException : Exception
    {
        public ResultException() {}
        public ResultException(string message) : base(message) {}
    }
}