using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace JapeWeb
{
    public partial class ResponseTree : WebComponent
    {
        private readonly Branch root;

        internal ResponseTree(PathString requestPath, Middleware middleware)
        {
            root = new Branch(requestPath, middleware);
        }

        internal override void Setup(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                Middleware.Result result = await Invoke(context);
                if (result.Prevented) { return; }
                await next.Invoke();
            });
        }

        public void Add(Response response)
        {
            if (response.fromRoot)
            {
                AddLow(response.requestPath, response.middleware);
            } 
            else 
            {
                AddLow(root.requestPath + response.requestPath, response.middleware);
            }
        }

        private void AddLow(PathString requestPath, Middleware response)
        {
            if (!requestPath.StartsWithSegments(root.requestPath, out PathString remainingPath))
            {
                throw new InvalidOperationException("Invalid Root");
            }

            root.Insert(remainingPath, response);
        }

        private async Task<Middleware.Result> Invoke(HttpContext context)
        {
            Result result = await root.Respond(context.Request.Path, context);
            return result.MiddlewareResult;
        }

        public void Write(Action<string> write) => root.WriteChildren(write, string.Empty);

        public static Response RootResponse(PathString requestPath) => new(true, requestPath, null);
        public static Response RootResponse(PathString requestPath, Middleware.Response response) => new(true, requestPath, new Middleware(response));
        public static Response RootResponse(PathString requestPath, Middleware.ResponseAsync response) => new(true, requestPath, new Middleware(response));
        
        public static Response RelativeResponse(PathString requestPath) => new(false, requestPath, null);
        public static Response RelativeResponse(PathString requestPath, Middleware.Response response) => new(false, requestPath, new Middleware(response));
        public static Response RelativeResponse(PathString requestPath, Middleware.ResponseAsync response) => new(false, requestPath, new Middleware(response));
    }
}
