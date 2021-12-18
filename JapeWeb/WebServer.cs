using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JapeCore;
using JapeHttp;
using JapeService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace JapeWeb
{
    public class WebServer : Service
    {
        public string Root => listener.Root;

        protected virtual string ServerName => "Kestrel";
        protected virtual string StaticDirectory => "./public";
        protected virtual string LandingPage => "./index.html";
        
        protected virtual bool Buffering => true;
        protected virtual bool Caching => true;

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

        private bool setupWebComponents;
        private readonly List<WebComponent> webComponents = new();

        private WebListener listener;

        public WebServer(int http, int https) : base(http, https)
        {
            this.http = http;
            this.https = https;
        }

        protected override Listener ServiceListener()
        {
            listener = new WebListener(StaticDirectory);

            if (Buffering) { listener.EnableBuffering(); }
            if (Caching) { listener.EnableCaching(); }

            listener.UseSetup(SetupLow);
            listener.UseServices(ServicesLow);

            return listener;
        }

        protected string ReadStaticFile(string path) => listener.ReadStaticFile(path);
        protected async Task<string> ReadStaticFileAsync(string path) => await listener.ReadStaticFileAsync(path);

        protected string ReadServerFile(string path) => listener.ReadFile(path);
        protected async Task<string> ReadServerFileAsync(string path) => await listener.ReadFileAsync(path);

        protected Templater CreateTemplater(string staticPath)
        {
            if (!Directory.Exists(Path.Combine(listener.Root, SystemPath.Format(staticPath))))
            {
                throw new DirectoryNotFoundException("Template directory does not exist");
            }
            
            Templater templater = new(staticPath, ReadServerFile, ReadServerFileAsync);
            return templater;
        }

        public Middleware Use(Middleware.Response response)
        {
            if (!setupWebComponents) { WebComponentException.SetupException(); }

            Middleware middleware = new(response);
            return middleware;
        }

        public Middleware UseAsync(Middleware.ResponseAsync response)
        {
            if (!setupWebComponents) { WebComponentException.SetupException(); }

            Middleware middleware = new(response);
            return middleware;
        }

        public Routing Route(PathString requestPath, string staticPath)
        {
            if (!setupWebComponents) { WebComponentException.SetupException(); }

            return RouteLow(requestPath, staticPath, null);
        }

        public Routing Route(PathString requestPath, string staticPath, Middleware.Response response)
        {
            if (!setupWebComponents) { WebComponentException.SetupException(); }

            return RouteLow(requestPath, staticPath, new Middleware(response));
        }

        public Routing Route(PathString requestPath, string staticPath, Middleware.ResponseAsync response)
        {
            if (!setupWebComponents) { WebComponentException.SetupException(); }

            return RouteLow(requestPath, staticPath, new Middleware(response));
        }

        private Routing RouteLow(PathString requestPath, string staticPath, Middleware middleware)
        {
            if (!setupWebComponents) { WebComponentException.SetupException(); }

            Routing routing = new(requestPath, Path.Combine(Root, SystemPath.Format(staticPath)), middleware);
            return routing;
        }

        public Mapping Map(PathString requestPath, string responsePath, Mapping.Read read)
        {
            if (!setupWebComponents) { WebComponentException.SetupException(); }

            Mapping mapping = new(Mapping.Method.Any, requestPath, responsePath, read);
            return mapping;
        }

        public Mapping MapGet(PathString requestPath, string responsePath, Mapping.Read read)
        {
            if (!setupWebComponents) { WebComponentException.SetupException(); }

            Mapping mapping = new(Mapping.Method.Get, requestPath, responsePath, read);
            return mapping;
        }

        public Mapping MapPost(PathString requestPath, string responsePath, Mapping.Read read)
        {
            if (!setupWebComponents) { WebComponentException.SetupException(); }

            Mapping mapping = new(Mapping.Method.Post, requestPath, responsePath, read);
            return mapping;
        }

        public ResponseTree CreateResponseTree(PathString rootPath, Middleware.Response rootResponse, params ResponseTree.Response[] responses)
        {
            if (!setupWebComponents) { WebComponentException.SetupException(); }

            return CreateResponseTreeLow(rootPath, new Middleware(rootResponse), responses);
        }

        public ResponseTree CreateResponseTree(PathString rootPath, Middleware.ResponseAsync rootResponse, params ResponseTree.Response[] responses)
        {
            if (!setupWebComponents) { WebComponentException.SetupException(); }

            return CreateResponseTreeLow(rootPath, new Middleware(rootResponse), responses);
        }

        public ResponseTree CreateResponseTree(PathString rootPath, params ResponseTree.Response[] responses)
        {
            if (!setupWebComponents) { WebComponentException.SetupException(); }

            return CreateResponseTreeLow(rootPath, null, responses);
        }

        private ResponseTree CreateResponseTreeLow(PathString rootPath, Middleware rootMiddleware, params ResponseTree.Response[] responses)
        {
            if (!setupWebComponents) { WebComponentException.SetupException(); }

            ResponseTree tree = new(rootPath, rootMiddleware);
            foreach (ResponseTree.Response response in responses)
            {
                tree.Add(response);
            }

            return tree;
        }

        private void SetupLow(IApplicationBuilder app)
        {
            app.UseSession();
            Setup(app);
            SetupComponents(app);
            SetupLandingPage(app);
        }

        private void ServicesLow(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddSession(SessionLow);
            Services(services);
        }

        private void SetupComponents(IApplicationBuilder app)
        {
            setupWebComponents = true;
            IEnumerator<WebComponent> components = Components();
            while (components.MoveNext())
            {
                components.Current.Setup(app);
                webComponents.Add(components.Current);
            }
            setupWebComponents = false;
        }

        private void SetupLandingPage(IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    string data = await ReadStaticFileAsync(LandingPage);
                    await HttpResponseWritingExtensions.WriteAsync(context.Response, data);
                });
            });
        }

        private void SessionLow(SessionOptions options)
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.Name = $"{ServerName}.Session";
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.IdleTimeout = TimeSpan.FromMinutes(10);
            Session(options);
        }

        protected virtual void Setup(IApplicationBuilder app) { }
        protected virtual void Services(IServiceCollection services) {}
        protected virtual void Session(SessionOptions options) {}

        protected virtual IEnumerator<WebComponent> Components() { yield return null; }
    }
}
