using System.Threading.Tasks;
using JapeHttp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace JapeWeb
{
    public class Mapping : IWebComponent
    {
        public enum Method { Get, Post, Any }

        private readonly Method method;

        private readonly PathString requestPath;
        private readonly string responsePath;

        private readonly Read read;
    
        public delegate Task<string> Read(string path);

        internal Mapping(Method method, PathString requestPath, string responsePath, Read read)
        {
            this.method = method;
            this.requestPath = requestPath;
            this.responsePath = responsePath;
            this.read = read;
        }

        internal void Build(IEndpointRouteBuilder builder)
        {
            switch (method)
            {
                case Method.Get:
                    builder.MapGet(requestPath, Respond);
                    break;
                case Method.Post:
                    builder.MapPost(requestPath, Respond);
                    break;
                case Method.Any:
                    builder.Map(requestPath, Respond);
                    break;
            }
        }

        private async Task Respond(HttpContext context) 
        {
            string data = await read.Invoke(responsePath);
            context.Response.StatusCode = (int)Status.SuccessCode.Ok;
            await context.Response.Write(data);
            await context.Response.CompleteAsync();
        }
    }
}
