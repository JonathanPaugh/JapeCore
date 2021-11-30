using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace JapeHttp
{
    public class Middleware
    {
        private readonly Response middlewareSync;
        private readonly ResponseAsync middlewareAsync;

        public delegate Result Response(HttpContext context);
        public delegate Task<Result> ResponseAsync(HttpContext context);

        public bool Skip { get; set; }

        private Middleware(Response middleware)
        {
            middlewareSync = middleware;
        }

        private Middleware(ResponseAsync middleware)
        {
            middlewareAsync = middleware;
        }

        public async Task<Result> Invoke(HttpContext context)
        {
            if (Skip)
            {
                return await Task.FromResult(Result.Skip());
            }

            if (middlewareSync != null)
            {
                Result result = middlewareSync.Invoke(context);
                if (result == null) { throw new MiddlewareException("Null Result"); }
                return await Task.FromResult(result);
            }

            if (middlewareAsync != null)
            {
                Result result = await middlewareAsync.Invoke(context);
                if (result == null) { throw new MiddlewareException("Null Result"); }
                return result;
            }

            throw new MiddlewareException("Invalid State");
        }

        public static Middleware Use(Response middleware) => new(middleware);
        public static Middleware UseAsync(ResponseAsync middleware) => new(middleware);

        public class Result
        {
            internal bool Prevented { get; }
            internal bool Skipped { get; private set; }

            private Result(bool prevented)
            {
                Prevented = prevented;
            }

            public static Result Prevent() => new(true);
            public static Result Continue() => new(false);

            internal static Result Skip()
            {
                return new Result(false)
                {
                    Skipped = true
                };
            }
        }
    }
}
