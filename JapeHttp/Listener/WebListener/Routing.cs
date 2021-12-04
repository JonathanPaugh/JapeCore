using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace JapeHttp
{
    public class Routing
    {
        public enum Method { Get, Post, Any }

        private readonly string path;
        private readonly Response response;
        private readonly Method method;

        public delegate Task<Request.Result> Response(WebListener.Request request);

        private Routing(string path, Response response, Method method)
        {
            this.path = path;
            this.response = response;
            this.method = method;
        }

        internal void Build(IEndpointRouteBuilder builder)
        {
            switch (method)
            {
                case Method.Get:
                    builder.MapGet(path, Respond);
                    break;
                case Method.Post:
                    builder.MapPost(path, Respond);
                    break;
                case Method.Any:
                    builder.Map(path, Respond);
                    break;
            }
        }

        private async Task Respond(HttpContext context) => await response.Invoke(new WebListener.Request(context.Request, context.Response));

        public static Routing MapGet(string path, Response response) => new(path, response, Method.Get);
        public static Routing MapPost(string path, Response response) => new(path, response, Method.Post);
        public static Routing Map(string path, Response response) => new(path, response, Method.Any);
    }
}
