using JapeCore;
using JapeHttp;
using JapeService;
using Microsoft.Extensions.Logging;

namespace JapeWeb
{
    public class WebServer : Service
    {
        protected override string StartString => $"{base.StartString}: {http}";

        private readonly int http;
        private readonly int https;

        private Logger accessLogger = Log.Create("access.log");

        public WebServer(int http, int https) : base(http, https)
        {
            this.http = http;
            this.https = https;
        }

        protected override Listener ServiceListener()
        {
            WebListener listener = new();

            #pragma warning disable 1998
            listener.Use(async (context) => 
            {
                accessLogger.Write($"({context.Connection.RemoteIpAddress.MapToIPv4()}) => Request: {context.Request.Path}");
            });
            #pragma warning restore 1998

            return listener;
        }
    }
}
