using System;

namespace JapeWeb
{
    public class WebComponentException : Exception
    {
        private const string SetupMessage = "Unable to create web component outside of setup function";

        private WebComponentException() {}
        private WebComponentException(string message) : base(message) {}

        public static WebComponentException SetupException() => throw new WebComponentException(SetupMessage);
    }
}
