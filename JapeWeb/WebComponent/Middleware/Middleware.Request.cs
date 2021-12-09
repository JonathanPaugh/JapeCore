using System.Threading.Tasks;
using JapeCore;
using JapeHttp;
using Microsoft.AspNetCore.Http;

namespace JapeWeb
{
    public partial class Middleware
    {
        public class Request : JapeHttp.Request, ICloseableRequest<Middleware.Result>
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

            public async Task<Middleware.Result> Complete(Status.SuccessCode code)
            {
                await Close((int)code);
                return Middleware.Result.Prevent;
            }

            public async Task<Middleware.Result> Complete(Status.SuccessCode code, string data)
            {
                await Close((int)code, data);
                return Middleware.Result.Prevent;
            }

            public async Task<Middleware.Result> Complete(Status.SuccessCode code, JsonData data)
            {
                await Close((int)code, data);
                return Middleware.Result.Prevent;
            }

            public async Task<Middleware.Result> Abort(Status.ErrorCode code)
            {
                await Close((int)code);
                return Middleware.Result.Prevent;
            }

            #pragma warning disable CA1822 // Mark members as static
            public Middleware.Result Next() => Middleware.Result.Next;
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