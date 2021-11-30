using System;
using System.IO;
using System.Threading.Tasks;
using JapeCore;
using JapeHttp;
using JapeService;

namespace JapeWeb
{
    public class WebServer : Service
    {
        protected virtual string StaticDirectory => "./public";
        protected virtual string LandingPage => "./index.html";

        protected override string StartString
        {
            get
            {
                string message = $"{base.StartString}";
                if (http > 0)
                {
                    message += Environment.NewLine;
                    message += Log.Stamp($"Port: {http}");
                }

                if (https > 0)
                {
                    message += Environment.NewLine;
                    message += Log.Stamp($"Secure Port: {https}");
                }

                return message;
            }
        }

        private readonly int http;
        private readonly int https;

        private WebListener listener;

        public WebServer(int http, int https) : base(http, https)
        {
            this.http = http;
            this.https = https;
        }

        protected async Task<string> ReadStaticFile(string path) => await listener.ReadStaticFile(path);
        protected async Task<string> ReadServerFile(string path) => await listener.ReadFile(path);

        protected void Use(Middleware middleware) => listener.Use(middleware);
        protected void Route(Routing routing) => listener.Route(routing);

        protected Templater CreateTemplater(string path)
        {
            if (!Directory.Exists(Path.Combine(listener.BasePath, VirtualPath.Format(path))))
            {
                throw new DirectoryNotFoundException("Template directory does not exist");
            }
            
            return new Templater(path, ReadServerFile);
        }

        protected override Listener ServiceListener()
        {
            listener = new WebListener(StaticDirectory, LandingPage);
            return listener;
        }
    }
}
