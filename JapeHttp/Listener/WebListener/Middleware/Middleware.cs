using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace JapeHttp
{
    public partial class Middleware
    {
        private readonly Response middlewareSync;
        private readonly ResponseAsync middlewareAsync;

        public delegate Request.Result Response(Request request);
        public delegate Task<Request.Result> ResponseAsync(Request request);

        public bool Skip { get; set; }

        private Middleware(Response middleware)
        {
            middlewareSync = middleware;
        }

        private Middleware(ResponseAsync middleware)
        {
            middlewareAsync = middleware;
        }

        public async Task<Request.Result> Invoke(HttpContext context)
        {
            if (Skip)
            {
                return await Task.FromResult(Request.Result.Skip());
            }

            Request request = await Request.Create(context.Request, context.Response);

            if (middlewareSync != null)
            {
                Request.Result result = middlewareSync.Invoke(request);
                if (result == null) { throw new ResultException("Null Result"); }
                return await Task.FromResult(result);
            }

            if (middlewareAsync != null)
            {
                Request.Result result = await middlewareAsync.Invoke(request);
                if (result == null) { throw new ResultException("Null Result"); }
                return result;
            }

            throw new Exception("Invalid Middleware State");
        }

        public static Middleware Use(Response middleware) => new(middleware);
        public static Middleware UseAsync(ResponseAsync middleware) => new(middleware);
    }
}
