﻿using System;
using System.IO;
using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JapeHttp
{
    public class Listener
    {
        private IHostBuilder builder;
        
        private Action<HttpContext> handler;

        private static string CertificateFile => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../ssl.pfx");

        public Listener(Action<HttpContext> handler)
        {
            this.handler = handler;

            builder = Host.CreateDefaultBuilder().ConfigureWebHostDefaults(builder =>
            {
                builder.Configure(Init);

                builder.ConfigureLogging(logger =>
                {
                    logger.SetMinimumLevel(LogLevel.Warning);
                });

                builder.UseKestrel(kestrel =>
                {
                    kestrel.ConfigureHttpsDefaults(https =>
                    {
                        https.CheckCertificateRevocation = false;
                        if (File.Exists(CertificateFile))
                        {
                            https.ServerCertificate = new X509Certificate2(CertificateFile);
                        }
                    });
                });
            });
        }

        public void CreateServer(int port)
        {
            builder.ConfigureWebHostDefaults(builder =>
            {
                builder.ConfigureKestrel(kestrel =>
                {
                    kestrel.ListenAnyIP(port);
                });
            });
        }

        public void CreateServerSecure(int port)
        {
            builder.ConfigureWebHostDefaults(builder =>
            {
                builder.ConfigureKestrel(kestrel =>
                {
                    kestrel.ListenAnyIP(port, options =>
                    {
                        options.UseHttps();
                    });
                });
            });
        }

        private void Init(IApplicationBuilder app)
        {
            #pragma warning disable 1998
            app.Run(async context =>
            {
                handler.Invoke(context);
            });
            #pragma warning restore 1998
        }

        public void Start()
        {
            Task.Run(() =>
            {
                IHost host = builder.UseConsoleLifetime().Build();
                host.Run();
                Environment.Exit(0);
            });
        }
    }
}
