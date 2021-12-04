using System.Threading.Tasks;
using JapeCore;
using Microsoft.AspNetCore.Http;

namespace JapeHttp
{
    public partial class Middleware
    {
        public partial class Request : JapeHttp.Request, ICloseableRequest<Request.Result>
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
        }
    }
}