namespace JapeHttp
{
    public static class Status
    {
        

        public enum SuccessCode
        {
            Ok = 200,
            Created = 201,
            Accepted = 202,
            Empty = 204
        }

        public enum ErrorCode
        {
            BadRequest = 400,
            Forbidden = 403,
            NotFound = 404,
            RequestTimeout = 408,
            ServerError = 500,
            NotImplemented = 501,
        }
    }
}
