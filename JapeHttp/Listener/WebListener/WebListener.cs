using System;
using System.IO;
using System.Threading.Tasks;
using JapeCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;

namespace JapeHttp
{
    public class WebListener : Listener
    {
        public enum AccessLogMode { None, LogOnly, ConsoleLog }

        public string Root => AppDomain.CurrentDomain.BaseDirectory;
        public string StaticPath => Path.Combine(Root, staticDirectory);

        public AccessLogMode AccessLogging { get; set; } = AccessLogMode.LogOnly;

        private readonly string staticDirectory = "./public";

        private readonly Logger accessLogger = Log.Create("access.log");

        private readonly PhysicalFileProvider baseFileProvider;
        private readonly PhysicalFileProvider staticFileProvider;

        private Action<IApplicationBuilder> setup;
        private Action<IServiceCollection> services;

        private bool buffering;
        private bool caching;

        public WebListener(string staticDirectory = null)
        {
            if (staticDirectory != null) { this.staticDirectory = staticDirectory; }

            if (!Directory.Exists(StaticPath))
            {
                Log.Write($"Static directory does not exist: {StaticPath}");
                Directory.CreateDirectory(StaticPath);
                Log.Write("Empty static directory created");
            }

            baseFileProvider = new PhysicalFileProvider(Root);
            staticFileProvider = new PhysicalFileProvider(StaticPath);
        }

        public string ReadFile(string path)
        {
            IFileInfo fileInfo = baseFileProvider.GetFileInfo(SystemPath.Format(path));
            if (!fileInfo.Exists) { return null; }
            return fileInfo.Read();
        }

        public async Task<string> ReadFileAsync(string path)
        {
            IFileInfo fileInfo = baseFileProvider.GetFileInfo(SystemPath.Format(path));
            if (!fileInfo.Exists) { return null; }
            return await fileInfo.ReadAsync();
        }

        public string ReadStaticFile(string path)
        {
            IFileInfo fileInfo = staticFileProvider.GetFileInfo(SystemPath.Format(path));
            if (!fileInfo.Exists) { return null; }
            return fileInfo.Read();
        }

        public async Task<string> ReadStaticFileAsync(string path)
        {
            IFileInfo fileInfo = staticFileProvider.GetFileInfo(SystemPath.Format(path));
            if (!fileInfo.Exists) { return null; }
            return await fileInfo.ReadAsync();
        }

        public void UseSetup(Action<IApplicationBuilder> setup)
        {
            if (PreventIfConstructed(nameof(UseSetup))) { return; }
            this.setup = setup;
        }

        public void UseServices(Action<IServiceCollection> services)
        {
            if (PreventIfConstructed(nameof(UseServices))) { return; }
            this.services = services;
        }

        public void EnableBuffering()
        {
            if (PreventIfConstructed(nameof(EnableBuffering))) { return; }
            buffering = true;
        }

        public void EnableCaching()
        {
            if (PreventIfConstructed(nameof(EnableCaching))) { return; }
            caching = true;
        }

        protected override void Services(IServiceCollection services)
        {
            this.services?.Invoke(services);
        }

        protected sealed override void Setup(IApplicationBuilder app)
        {
            app.UseRouting();

            app.Use(async (context, next) =>
            {
                await OnRequest(context);
                await next.Invoke();
            });

            setup?.Invoke(app);

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = staticFileProvider,
            });
        }

        private Task OnRequest(HttpContext context)
        {   
            LogAccess(context);

            if (buffering) { context.Request.EnableBuffering(); }
            if (!caching) { context.Response.Headers.AppendCommaSeparatedValues(HeaderNames.CacheControl, Request.NoCache); }
            
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
