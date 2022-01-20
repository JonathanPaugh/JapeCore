using System;

namespace JapeHttp
{
    public class ResponseException : Exception
    {
        public Status.ErrorCode StatusCode { get; }

        public ResponseException(Status.ErrorCode statusCode, Exception innerException) : base(GetMessage(statusCode), innerException)
        {
            StatusCode = statusCode;
        }

        private static string GetMessage(Status.ErrorCode statusCode) => $"Server response error: {statusCode} ({(int)statusCode})";
    }
}
