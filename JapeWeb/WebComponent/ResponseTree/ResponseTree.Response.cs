using Microsoft.AspNetCore.Http;

namespace JapeWeb
{
    public partial class ResponseTree
    {
        public class Response
        {
            internal readonly bool fromRoot;
            internal readonly PathString requestPath;
            internal readonly Middleware middleware;

            internal Response(bool fromRoot, PathString requestPath, Middleware middleware)
            {
                this.fromRoot = fromRoot;
                this.requestPath = requestPath;
                this.middleware = middleware;
            }
        }
    }
}