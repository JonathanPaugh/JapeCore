using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace JapeWeb
{
    public class Routing : WebComponent
    {
        private bool UseMiddleware => middleware != null;

        private readonly PathString requestPath;
        private readonly PhysicalFileProvider fileProvider;
        private readonly Middleware middleware;

        internal Routing(string requestPath, string staticPath, Middleware middleware)
        {
            this.requestPath = requestPath;
            fileProvider = new PhysicalFileProvider(staticPath);
            this.middleware = middleware;
        }

        internal override void Setup(IApplicationBuilder app)
        {
            if (UseMiddleware)
            {
                app.Use(async (context, next) =>
                {
                    Middleware.Result result = await Invoke(context);
                    if (result.Prevented) { return; }
                    await next.Invoke();
                });
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = requestPath,
                FileProvider = fileProvider
            });
        }

        private async Task<Middleware.Result> Invoke(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments(requestPath)) { return await Task.FromResult(Middleware.Result.Next); }
            return await Middleware.InvokeExternal(middleware, context);
        }
    }
}
