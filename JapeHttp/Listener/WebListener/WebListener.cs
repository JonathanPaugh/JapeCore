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
    public partial class WebListener : Listener
    {
        public enum AccessLogMode { None, LogOnly, ConsoleLog }

        public string BasePath => AppDomain.CurrentDomain.BaseDirectory;
        public string StaticPath => Path.Combine(BasePath, staticDirectory);

        private readonly string staticDirectory = "public";
        private readonly string landingPage = "index.html";

        private readonly Logger accessLogger = Log.Create("access.log");

        private readonly List<Middleware> middlewareCollection = new();
        private readonly List<Routing> routingCollection = new();

        private readonly PhysicalFileProvider baseFileProvider;
        private readonly PhysicalFileProvider staticFileProvider;

        public AccessLogMode AccessLogging { get; set; } = AccessLogMode.LogOnly;

        public WebListener(string staticDirectory = null, string landingPage = null)
        {
            if (staticDirectory != null) { this.staticDirectory = staticDirectory; }
            if (landingPage != null) { this.landingPage = landingPage; }

            if (!Directory.Exists(StaticPath))
            {
                Log.Write($"Static directory does not exist: {StaticPath}");
                Directory.CreateDirectory(StaticPath);
                Log.Write("Empty static directory created");
            }

            baseFileProvider = new PhysicalFileProvider(BasePath);
            staticFileProvider = new PhysicalFileProvider(StaticPath);
        }

        public void Use(Middleware middleware)
        {
            if (PreventIfRunning("UseMiddleware")) { return; }
            middlewareCollection.Add(middleware);
        }

        public void Route(Routing routing)
        {
            if (PreventIfRunning(nameof(Route))) { return; }
            routingCollection.Add(routing);
        }

        public async Task<string> ReadFile(string path)
        {
            IFileInfo fileInfo = baseFileProvider.GetFileInfo(VirtualPath.Format(path));
            if (!fileInfo.Exists) { return null; }
            return await fileInfo.Read();
        }

        public async Task<string> ReadStaticFile(string path)
        {
            IFileInfo fileInfo = staticFileProvider.GetFileInfo(VirtualPath.Format(path));
            if (!fileInfo.Exists) { return null; }
            return await fileInfo.Read();
        }

        protected sealed override void Setup(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                await OnRequest(context);
                await next.Invoke();
            });

            SetupMiddleware(app);

            app.UseRouting();

            SetupLandingPage(app);

            SetupRouting(app);

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = staticFileProvider,
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
        
        private void SetupRouting(IApplicationBuilder app)
        {
            foreach (Routing routing in routingCollection)
            {
                app.UseEndpoints(routing.Build);
            }
        }

        private void SetupLandingPage(IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    string data = await ReadStaticFile(landingPage);
                    await context.Response.WriteAsync(data);
                });
            });
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
