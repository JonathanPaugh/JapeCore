using System;
using System.Threading.Tasks;
using JapeHttp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace JapeWeb
{
    public partial class Middleware : WebComponent
    {
        public bool Skip { get; set; }

        private readonly Response response;
        private readonly ResponseAsync responseAsync;

        public delegate Result Response(Request request);
        public delegate Task<Result> ResponseAsync(Request request);
        public delegate T RequestLookup<out T>(Request request);

        internal Middleware(Response response) { this.response = response; }
        internal Middleware(ResponseAsync response) { responseAsync = response; }

        internal override void Setup(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                Result result = await Invoke(context);
                if (result.Prevented) { return; }
                await next.Invoke();
            });
        }

        private async Task<Result> Invoke(HttpContext context)
        {
            if (Skip)
            {
                return await Task.FromResult(Result.Skip);
            }

            Request request = await Request.Create(context);

            if (response != null)
            {
                Result result = response.Invoke(request);
                if (result == null) { throw new ResultException("Null Result"); }
                return await Task.FromResult(result);
            }

            if (responseAsync != null)
            {
                Result result = await responseAsync.Invoke(request);
                if (result == null) { throw new ResultException("Null Result"); }
                return result;
            }

            throw new Exception("Invalid Middleware State");
        }

        internal static async Task<Result> InvokeExternal(Middleware middleware, HttpContext context) => await middleware.Invoke(context);
    }
}
