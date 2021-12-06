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

        public string Root => AppDomain.CurrentDomain.BaseDirectory;
        public string StaticPath => Path.Combine(Root, staticDirectory);

        private readonly string staticDirectory = "public";
        private readonly string landingPage = "index.html";

        private readonly Logger accessLogger = Log.Create("access.log");

        private readonly PhysicalFileProvider baseFileProvider;
        private readonly PhysicalFileProvider staticFileProvider;

        private readonly Action<IApplicationBuilder> setup;

        public AccessLogMode AccessLogging { get; set; } = AccessLogMode.LogOnly;

        public WebListener(string staticDirectory = null, string landingPage = null, Action<IApplicationBuilder> setup = null)
        {
            if (staticDirectory != null) { this.staticDirectory = staticDirectory; }
            if (landingPage != null) { this.landingPage = landingPage; }

            this.setup = setup;

            if (!Directory.Exists(StaticPath))
            {
                Log.Write($"Static directory does not exist: {StaticPath}");
                Directory.CreateDirectory(StaticPath);
                Log.Write("Empty static directory created");
            }

            baseFileProvider = new PhysicalFileProvider(Root);
            staticFileProvider = new PhysicalFileProvider(StaticPath);
        }

        public async Task<string> ReadFile(string path)
        {
            IFileInfo fileInfo = baseFileProvider.GetFileInfo(SystemPath.Format(path));
            if (!fileInfo.Exists) { return null; }
            return await fileInfo.Read();
        }

        public async Task<string> ReadStaticFile(string path)
        {
            IFileInfo fileInfo = staticFileProvider.GetFileInfo(SystemPath.Format(path));
            if (!fileInfo.Exists) { return null; }
            return await fileInfo.Read();
        }

        protected sealed override void Setup(IApplicationBuilder app)
        {
            app.UseRouting();

            app.Use(async (context, next) =>
            {
                await OnRequest(context);
                await next.Invoke();
            });

            SetupLandingPage(app);

            setup?.Invoke(app);

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = staticFileProvider,
            });
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
            context.Request.EnableBuffering();
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
