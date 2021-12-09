using JapeCore;
using Microsoft.Extensions.FileProviders;

namespace JapeWeb
{
    public class Routing : IWebComponent
    {
        internal readonly string requestPath;
        internal readonly PhysicalFileProvider fileProvider;

        public Routing(string requestPath, string staticPath)
        {
            this.requestPath = requestPath;
            fileProvider = new PhysicalFileProvider(staticPath);
        }
    }
}
