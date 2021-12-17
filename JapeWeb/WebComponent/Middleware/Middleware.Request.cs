using System.Linq;
using System.Threading.Tasks;
using JapeCore;
using JapeHttp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace JapeWeb
{
    public partial class Middleware
    {
        public class Request : JapeHttp.Request, ICloseableRequest<Result>
        {
            public PathString Path => request.Path;

            public ISession Session { get; }
            public string Data { get; }

            public bool Cache
            {
                get => response.IsCached();
                set
                {
                    if (value)
                    {
                        StringValues values = response.Headers.GetCommaSeparatedValues(HeaderNames.CacheControl);

                        if (values == StringValues.Empty) { return; }

                        response.Headers.Remove(HeaderNames.CacheControl);
                        values = values.Where(value => value != NoCache).ToArray();

                        if (values == StringValues.Empty) { return; }
                        
                        response.Headers.Add(HeaderNames.CacheControl, values);
                    } 
                    else
                    {
                        if (!Cache) { return; }
                        response.Headers.AppendCommaSeparatedValues(HeaderNames.CacheControl, NoCache);
                    }
                }
            }

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

            private Request(HttpRequest request, HttpResponse response, ISession session, string data) : base(request, response)
            {
                Session = session;
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

            internal static async Task<Request> Create(HttpContext context)
            {
                string data = null;

                HttpRequest request = context.Request;

                if (request.ContentLength > 0)
                {
                    data = await request.ReadAsync(true);
                    request.Body.Rewind();
                }

                return new Request(request, context.Response, context.Session, data);
            }
        }
    }
}