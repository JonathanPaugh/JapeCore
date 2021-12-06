using System;
using System.Threading.Tasks;
using JapeHttp;
using Microsoft.AspNetCore.Http;

namespace JapeWeb
{
    public partial class Middleware
    {
        public bool Skip { get; set; }

        private readonly Response middlewareSync;
        private readonly ResponseAsync middlewareAsync;

        public delegate Result Response(Request request);
        public delegate Task<Result> ResponseAsync(Request request);

        internal Middleware(Response middleware)
        {
            middlewareSync = middleware;
        }

        internal Middleware(ResponseAsync middleware)
        {
            middlewareAsync = middleware;
        }

        internal async Task<Result> Invoke(HttpContext context)
        {
            if (Skip)
            {
                return await Task.FromResult(Result.Skip);
            }

            Request request = await Request.Create(context.Request, context.Response);

            if (middlewareSync != null)
            {
                Result result = middlewareSync.Invoke(request);
                if (result == null) { throw new ResultException("Null Result"); }
                return await Task.FromResult(result);
            }

            if (middlewareAsync != null)
            {
                Result result = await middlewareAsync.Invoke(request);
                if (result == null) { throw new ResultException("Null Result"); }
                return result;
            }

            throw new Exception("Invalid Middleware State");
        }
    }
}
