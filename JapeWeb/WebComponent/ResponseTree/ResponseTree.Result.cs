namespace JapeWeb
{
    public partial class ResponseTree
    {
        internal class Result : JapeHttp.Request.Result
        {
            internal Middleware.Result MiddlewareResult { get; }
            internal bool Complete { get; }

            internal static Result WrongPath => new(Middleware.Result.Next, false);
            internal static Result NotFound => new(Middleware.Result.Next, true);
            internal static Result Found(Middleware.Result middlewareResult) => new(middlewareResult, true);

            private Result(Middleware.Result result, bool complete)
            {
                MiddlewareResult = result;
                Complete = complete;
            }
        }
    }
}