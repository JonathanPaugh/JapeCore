using System;

namespace JapeService.Responder
{
    public class ResponderDataException : Exception
    {
        public ResponderDataException(Exception exception) : base(exception.Message, exception) {}
    }
}
