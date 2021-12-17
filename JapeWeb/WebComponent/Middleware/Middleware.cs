using System;
using System.Threading.Tasks;
using JapeHttp;
using Microsoft.AspNetCore.Http;

namespace JapeWeb
{
    public partial class Middleware : IWebComponent
    {
        public bool Skip { get; set; }

        private readonly Response response;
        private readonly ResponseAsync responseAsync;

        public delegate Result Response(Request request);
        public delegate Task<Result> ResponseAsync(Request request);
        public delegate T RequestLookup<out T>(Request request);

        internal Middleware(Response response) { this.response = response; }
        internal Middleware(ResponseAsync response) { responseAsync = response; }

        internal async Task<Result> Invoke(HttpContext context)
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
    }
}
