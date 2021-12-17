using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace JapeWeb
{
    public class Routing : IWebComponent
    {
        internal bool UseMiddleware => middleware != null;

        internal readonly PathString requestPath;
        internal readonly PhysicalFileProvider fileProvider;

        private readonly Middleware middleware;

        internal Routing(string requestPath, string staticPath, Middleware middleware)
        {
            this.requestPath = requestPath;
            fileProvider = new PhysicalFileProvider(staticPath);
            this.middleware = middleware;
        }

        internal async Task<Middleware.Result> Invoke(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments(requestPath)) { return await Task.FromResult(Middleware.Result.Next); }
            return await middleware.Invoke(context);
        }
    }
}
