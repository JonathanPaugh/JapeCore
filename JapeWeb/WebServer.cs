using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JapeCore;
using JapeHttp;
using JapeService;
using JapeWeb.WebComponent;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

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

        private readonly List<Middleware> middlewares = new();
        private readonly List<Mapping> mappings = new();
        private readonly List<Routing> routings = new();
        private readonly List<ResponseTree> responseTrees = new();
        private readonly List<Authenticator> authenticators = new();
        private readonly List<Templater> templaters = new();

        private WebListener listener;

        public WebServer(int http, int https) : base(http, https)
        {
            this.http = http;
            this.https = https;

            SetupComponents();
        }

        protected override Listener ServiceListener()
        {
            listener = new WebListener(StaticDirectory, LandingPage);

            if (Buffering) { listener.EnableBuffering(); }
            if (Caching) { listener.EnableCaching(); }

            listener.UseSetup(SetupLow);
            listener.UseSession(SessionLow);

            return listener;
        }

        protected async Task<string> ReadStaticFile(string path) => await listener.ReadStaticFile(path);
        protected async Task<string> ReadServerFile(string path) => await listener.ReadFile(path);

        public Middleware Use(Middleware.Response response)
        {
            if (PreventIfRunning($"{nameof(Use)}Middleware")) { throw new WebComponentException(); }

            Middleware middleware = new(response);
            middlewares.Add(middleware);
            return middleware;
        }

        public Middleware UseAsync(Middleware.ResponseAsync response)
        {
            if (PreventIfRunning($"{nameof(UseAsync)}Middleware")) { throw new WebComponentException(); }

            Middleware middleware = new(response);
            middlewares.Add(middleware);
            return middleware;
        }

        public Routing Route(PathString requestPath, string staticPath)
        {
            if (PreventIfRunning(nameof(Route))) { throw new WebComponentException(); }

            return RouteLow(requestPath, staticPath, null);
        }

        public Routing Route(PathString requestPath, string staticPath, Middleware.Response response)
        {
            if (PreventIfRunning(nameof(Route))) { throw new WebComponentException(); }

            return RouteLow(requestPath, staticPath, new Middleware(response));
        }

        public Routing Route(PathString requestPath, string staticPath, Middleware.ResponseAsync response)
        {
            if (PreventIfRunning(nameof(Route))) { throw new WebComponentException(); }

            return RouteLow(requestPath, staticPath, new Middleware(response));
        }

        private Routing RouteLow(PathString requestPath, string staticPath, Middleware middleware)
        {
            if (PreventIfRunning(nameof(RouteLow))) { throw new WebComponentException(); }

            Routing routing = new(requestPath, Path.Combine(Root, SystemPath.Format(staticPath)), middleware);
            routings.Add(routing);
            return routing;
        }

        public Mapping Map(PathString requestPath, string responsePath, Mapping.Read read)
        {
            if (PreventIfRunning(nameof(Map))) { throw new WebComponentException(); }

            Mapping mapping = new(Mapping.Method.Any, requestPath, responsePath, read);
            mappings.Add(mapping);
            return mapping;
        }

        public Mapping MapGet(PathString requestPath, string responsePath, Mapping.Read read)
        {
            if (PreventIfRunning(nameof(MapGet))) { throw new WebComponentException(); }

            Mapping mapping = new(Mapping.Method.Get, requestPath, responsePath, read);
            mappings.Add(mapping);
            return mapping;
        }

        public Mapping MapPost(PathString requestPath, string responsePath, Mapping.Read read)
        {
            if (PreventIfRunning(nameof(MapPost))) { throw new WebComponentException(); }

            Mapping mapping = new(Mapping.Method.Post, requestPath, responsePath, read);
            mappings.Add(mapping);
            return mapping;
        }

        public ResponseTree CreateResponseTree(PathString rootPath, Middleware.Response rootResponse, params ResponseTree.Response[] responses)
        {
            if (PreventIfRunning(nameof(CreateResponseTree))) { throw new WebComponentException(); }

            return CreateResponseTreeLow(rootPath, new Middleware(rootResponse), responses);
        }

        public ResponseTree CreateResponseTree(PathString rootPath, Middleware.ResponseAsync rootResponse, params ResponseTree.Response[] responses)
        {
            if (PreventIfRunning(nameof(CreateResponseTree))) { throw new WebComponentException(); }

            return CreateResponseTreeLow(rootPath, new Middleware(rootResponse), responses);
        }

        public ResponseTree CreateResponseTree(PathString rootPath, params ResponseTree.Response[] responses)
        {
            if (PreventIfRunning(nameof(CreateResponseTree))) { throw new WebComponentException(); }

            return CreateResponseTreeLow(rootPath, null, responses);
        }

        private ResponseTree CreateResponseTreeLow(PathString rootPath, Middleware rootMiddleware, params ResponseTree.Response[] responses)
        {
            if (PreventIfRunning(nameof(CreateResponseTreeLow))) { throw new WebComponentException(); }

            ResponseTree tree = new(rootPath, rootMiddleware);
            foreach (ResponseTree.Response response in responses)
            {
                tree.Add(response);
            }
            responseTrees.Add(tree);
            return tree;
        }

        public Authenticator CreateAuthenticator(Authenticator.Signup signup, 
                                                 Authenticator.Login login, 
                                                 Middleware.RequestLookup<string> getRequestUser, 
                                                 Middleware.RequestLookup<string> getRequestPassword,
                                                 AuthenticatorOptions options)
        {
            if (PreventIfRunning(nameof(CreateAuthenticator))) { throw new WebComponentException(); }

            Authenticator authenticator = new(signup, login, getRequestUser, getRequestPassword);

            CreateResponseTreeLow(options.RequestPath, 
                                  null, 
                                  ResponseTree.RelativeResponse(options.SignupPath, authenticator.ResponseSignup), 
                                  ResponseTree.RelativeResponse(options.LoginPath, authenticator.ResponseLogin));

            authenticators.Add(authenticator);
            return authenticator;
        }

        protected Templater CreateTemplater(string staticPath)
        {
            if (!Directory.Exists(Path.Combine(listener.Root, SystemPath.Format(staticPath))))
            {
                throw new DirectoryNotFoundException("Template directory does not exist");
            }
            
            Templater templater = new(staticPath, ReadServerFile);
            templaters.Add(templater);
            return templater;
        }

        private void SetupLow(IApplicationBuilder app)
        {
            Setup(app);
            SetupMiddleware(app);
            SetupResponseTree(app);
            SetupMapping(app);
            SetupRouting(app);
        }

        private void SetupComponents()
        {
            IEnumerator<IWebComponent> components = Components();
            while (components.MoveNext()) {}
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
                if (routing.UseMiddleware)
                {
                    app.Use(async (context, next) =>
                    {
                        Middleware.Result result = await routing.Invoke(context);
                        if (result.Prevented) { return; }
                        await next.Invoke();
                    });
                }

                app.UseStaticFiles(new StaticFileOptions
                {
                    RequestPath = routing.requestPath,
                    FileProvider = routing.fileProvider
                });
            }
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

        protected virtual void Setup(IApplicationBuilder app) {}
        protected virtual void Session(SessionOptions options) {}

        protected virtual IEnumerator<IWebComponent> Components() { yield return null; }

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
