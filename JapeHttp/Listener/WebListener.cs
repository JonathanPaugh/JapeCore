using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JapeCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace JapeHttp
{
    public class WebListener : Listener
    {
        public enum AccessLogMode { None, LogOnly, ConsoleLog }

        private readonly string staticFolder = "public";
        private string StaticPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, staticFolder);

        private readonly Logger accessLogger = Log.Create("access.log");
        private readonly List<Middleware> middlewareCollection = new();

        public AccessLogMode AccessLogging { get; set; } = AccessLogMode.LogOnly;

        public WebListener(string staticFolder = null)
        {
            if (staticFolder != null) { this.staticFolder = staticFolder; }
        }

        public void Use(Middleware middleware)
        {
            if (PreventIfRunning("UseMiddleware")) { return; }
            middlewareCollection.Add(middleware);
        }

        protected sealed override void Setup(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                await OnRequest(context);
                await next.Invoke();
            });

            SetupMiddleware(app);

            if (!Directory.Exists(StaticPath))
            {
                Log.Write($"Static directory does not exist: {StaticPath}");
                Directory.CreateDirectory(StaticPath);
                Log.Write("Empty static directory created");
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(StaticPath),
            });
        }

        private void SetupMiddleware(IApplicationBuilder app)
        {
            foreach (Middleware middleware in middlewareCollection)
            {
                app.Use(async (context, next) =>
                {
                    Middleware.Result result = await middleware.Invoke(context);
                    if (result.Prevented) { return; }
                    await next.Invoke();
                });
            }
        }

        private Task OnRequest(HttpContext context)
        {   
            LogAccess(context);
            return Task.CompletedTask;
        }

        private void LogAccess(HttpContext context)
        {
            string message = $"({context.Connection.RemoteIpAddress.MapToIPv4()}) => Request: {context.Request.Path}";

            switch (AccessLogging)
            {
                default: return;
                case AccessLogMode.LogOnly: accessLogger.Log(message); break;
                case AccessLogMode.ConsoleLog: accessLogger.Write(message); break;
            }
        }
    }
}
