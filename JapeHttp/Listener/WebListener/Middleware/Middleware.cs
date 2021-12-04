using System;
using System.Threading.Tasks;
using JapeCore;
using Microsoft.AspNetCore.Http;

namespace JapeHttp
{
    public class Middleware
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
                if (result == null) { throw new MiddlewareException("Null Result"); }
                return await Task.FromResult(result);
            }

            if (middlewareAsync != null)
            {
                Request.Result result = await middlewareAsync.Invoke(request);
                if (result == null) { throw new MiddlewareException("Null Result"); }
                return result;
            }

            throw new MiddlewareException("Invalid State");
        }

        public static Middleware Use(Response middleware) => new(middleware);
        public static Middleware UseAsync(ResponseAsync middleware) => new(middleware);

        public class Request : JapeHttp.Request, ICloseableRequest<Request.Result>
        {
            public PathString Path => request.Path;

            public string Data { get; }

            private JsonData json;
            public JsonData Json
            {
                get
                {
                    if (Data == null) { return null; }
                    if (json != null) { return json; }
                    json = new JsonData(Data);
                    return json;
                }
            }

            private Request(HttpRequest request, HttpResponse response, string data) : base(request, response)
            {
                Data = data;
            }

            public async Task<Result> Complete(Status.SuccessCode code)
            {
                await Close((int)code);
                return Result.Prevent;
            }

            public async Task<Result> Complete(Status.SuccessCode code, string data)
            {
                await Close((int)code, data);
                return Result.Prevent;
            }

            public async Task<Result> Complete(Status.SuccessCode code, JsonData data)
            {
                await Close((int)code, data);
                return Result.Prevent;
            }

            public async Task<Result> Abort(Status.ErrorCode code)
            {
                await Close((int)code);
                return Result.Prevent;
            }

            #pragma warning disable CA1822 // Mark members as static
            public Result Next() => Result.Next;
            #pragma warning restore CA1822 // Mark members as static

            internal static async Task<Request> Create(HttpRequest request, HttpResponse response)
            {
                string data = null;

                if (request.ContentLength > 0)
                {
                    data = await request.ReadStream();
                    request.Body.Rewind();
                }

                return new Request(request, response, data);

            }

            public new class Result : JapeHttp.Request.Result
            {
                internal bool Prevented { get; }
                internal bool Skipped { get; private init; }

                internal static Result Prevent => new(true);
                internal static Result Next => new(false);

                private Result(bool prevented)
                {
                    Prevented = prevented;
                }

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
}
