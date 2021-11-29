using System.Threading;
using System.Threading.Tasks;
using JapeHttp;
using JapeService;

namespace JapeWeb
{
    public class WebServer : Service
    {
        protected virtual string StaticDirectory => "public";
        protected virtual string LandingPage => "index.html";

        protected override string StartString => $"{base.StartString}: {http}";

        private readonly int http;
        private readonly int https;

        private WebListener listener;

        public WebServer(int http, int https) : base(http, https)
        {
            this.http = http;
            this.https = https;
        }

        protected void Use(Middleware middleware) => listener.Use(middleware);

        protected override Listener ServiceListener()
        {
            listener = new WebListener(StaticDirectory);
            return listener;
        }
    }
}
