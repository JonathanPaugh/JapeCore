using System;

namespace JapeWeb.WebComponent
{
    public class WebComponentException : Exception
    {
        public WebComponentException() {}
        public WebComponentException(string message) : base(message) {}
    }
}
