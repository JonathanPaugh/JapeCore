using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JapeCore;
using JapeHttp;
using JapeService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace JapeWeb
{
    public class WebServer : Service
    {
        public string Root => listener.Root;

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

        private readonly List<Middleware> middlewares = new();
        private readonly List<Mapping> mappings = new();
        private readonly List<Routing> routings = new();
        private readonly List<ResponseTree> responseTrees = new();

        private WebListener listener;

        public WebServer(int http, int https) : base(http, https)
        {
            this.http = http;
            this.https = https;
        }

        protected override Listener ServiceListener()
        {
            listener = new WebListener(StaticDirectory, LandingPage, SetupLow);
            return listener;
        }

        protected async Task<string> ReadStaticFile(string path) => await listener.ReadStaticFile(path);
        protected async Task<string> ReadServerFile(string path) => await listener.ReadFile(path);

        protected Templater CreateTemplater(string staticPath)
        {
            if (!Directory.Exists(Path.Combine(listener.Root, SystemPath.Format(staticPath))))
            {
                throw new DirectoryNotFoundException("Template directory does not exist");
            }
            
            return new Templater(staticPath, ReadServerFile);
        }

        public Middleware Use(Middleware.Response response)
        {
            if (PreventIfRunning($"{nameof(Use)}Middleware")) { return null; }

            Middleware middleware = new(response);
            middlewares.Add(middleware);
            return middleware;
        }

        public Middleware UseAsync(Middleware.ResponseAsync response)
        {
            if (PreventIfRunning($"{nameof(UseAsync)}Middleware")) { return null; }

            Middleware middleware = new(response);
            middlewares.Add(middleware);
            return middleware;
        }

        public Routing Route(PathString requestPath, string staticPath)
        {
            if (PreventIfRunning(nameof(Route))) { return null; }

            Routing routing = new(requestPath, Path.Combine(Root, SystemPath.Format(staticPath)));
            routings.Add(routing);
            return routing;
        }

        public Mapping Map(PathString requestPath, string responsePath, Mapping.Read read)
        {
            if (PreventIfRunning(nameof(Map))) { return null; }

            Mapping mapping = new(Mapping.Method.Any, requestPath, responsePath, read);
            mappings.Add(mapping);
            return mapping;
        }

        public Mapping MapGet(PathString requestPath, string responsePath, Mapping.Read read)
        {
            if (PreventIfRunning(nameof(MapGet))) { return null; }

            Mapping mapping = new(Mapping.Method.Get, requestPath, responsePath, read);
            mappings.Add(mapping);
            return mapping;
        }

        public Mapping MapPost(PathString requestPath, string responsePath, Mapping.Read read)
        {
            if (PreventIfRunning(nameof(MapPost))) { return null; }

            Mapping mapping = new(Mapping.Method.Post, requestPath, responsePath, read);
            mappings.Add(mapping);
            return mapping;
        }

        public ResponseTree CreateResponseTree(PathString rootPath, Middleware.Response rootResponse, params ResponseTree.Response[] responses)
        {
            if (PreventIfRunning(nameof(CreateResponseTree))) { return null; }

            return CreateResponseTreeLow(rootPath, new Middleware(rootResponse), responses);
        }

        public ResponseTree CreateResponseTree(PathString rootPath, Middleware.ResponseAsync rootResponse, params ResponseTree.Response[] responses)
        {
            if (PreventIfRunning(nameof(CreateResponseTree))) { return null; }

            return CreateResponseTreeLow(rootPath, new Middleware(rootResponse), responses);
        }

        public ResponseTree CreateResponseTree(PathString rootPath, params ResponseTree.Response[] responses)
        {
            if (PreventIfRunning(nameof(CreateResponseTree))) { return null; }

            return CreateResponseTreeLow(rootPath, null, responses);
        }

        private ResponseTree CreateResponseTreeLow(PathString rootPath, Middleware rootMiddleware, params ResponseTree.Response[] responses)
        {
            if (PreventIfRunning(nameof(CreateResponseTree))) { return null; }

            ResponseTree tree = new(rootPath, rootMiddleware);
            foreach (ResponseTree.Response response in responses)
            {
                tree.Add(response);
            }
            responseTrees.Add(tree);
            return tree;
        }

        private void SetupLow(IApplicationBuilder app)
        {
            SetupMiddleware(app);
            SetupResponseTree(app);
            SetupMapping(app);
            SetupRouting(app);
            Setup(app);
        }

        private void SetupMiddleware(IApplicationBuilder app)
        {
            foreach (Middleware middleware in middlewares)
            {
                app.Use(async (context, next) =>
                {
                    Middleware.Result result = await middleware.Invoke(context);
                    if (result.Prevented) { return; }
                    await next.Invoke();
                });
            }
        }

        private void SetupResponseTree(IApplicationBuilder app)
        {
            foreach (ResponseTree tree in responseTrees)
            {
                app.Use(async (context, next) =>
                {
                    Middleware.Result result = await tree.Invoke(context);
                    if (result.Prevented) { return; }
                    await next.Invoke();
                });
            }
        }

        private void SetupMapping(IApplicationBuilder app)
        {
            foreach (Mapping mapping in mappings)
            {
                app.UseEndpoints(mapping.Build);
            }
        }

        private void SetupRouting(IApplicationBuilder app)
        {
            foreach (Routing routing in routings)
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    RequestPath = routing.requestPath,
                    FileProvider = routing.fileProvider
                });
            }
        }

        protected virtual void Setup(IApplicationBuilder app) {}

        protected bool PreventIfRunning(string name) => Prevention("running", name, () => listener.Running);
        protected bool PreventIfIdle(string name) => Prevention("idle", name, () => !listener.Running);

        private static bool Prevention(string message, string name, Func<bool> condition)
        {
            if (condition())
            {
                Log.Write($"Unable to execute action while web server is {message}: {name}");
                return true;
            }

            return false;
        }
    }
}
