using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace JapeHttp
{
    public class WebListener : Listener
    {
        private readonly string staticFolder = "public";
        private string StaticPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, staticFolder);

        private List<Func<HttpContext, Task>> middleware = new();

        public WebListener(string staticFolder = null)
        {
            if (staticFolder != null) { this.staticFolder = staticFolder; }
        }

        public void Use(Func<HttpContext, Task> middleware)
        {
            this.middleware.Add(middleware);
        }

        protected sealed override void Setup(IApplicationBuilder app)
        {
            Directory.CreateDirectory(StaticPath);

            foreach (Func<HttpContext, Task> module in middleware)
            {
                app.Use(async (context, next) =>
                {
                    await module(context);
                    await next.Invoke();
                });
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(StaticPath),
            });
        }
    }
}
