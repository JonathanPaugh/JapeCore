using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace JapeHttp
{
    public class Middleware
    {
        private readonly Func<HttpContext, Result> middlewareSync;
        private readonly Func<HttpContext, Task<Result>> middlewareAsync;

        public bool Skip { get; set; }

        public Middleware(Func<HttpContext, Result> middleware)
        {
            middlewareSync = middleware;
        }

        public Middleware(Func<HttpContext, Task<Result>> middleware)
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
                return await Task.FromResult(result);
            }

            if (middlewareAsync != null)
            {
                return await middlewareAsync.Invoke(context);
            }

            throw new Exception("Invalid Middleware");
        }

        public static Middleware Use(Func<HttpContext, Result> middleware) => new(middleware);
        public static Middleware UseAsync(Func<HttpContext, Task<Result>> middleware) => new(middleware);

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
