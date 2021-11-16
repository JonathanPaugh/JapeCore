using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JapeHttp
{
    public class Listener
    {
        private IHostBuilder builder;
        
        private Func<HttpContext, Task> onRequest;

        private static string CertificateFile => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../ssl.pfx");

        public Listener(Func<HttpContext, Task> onRequest)
        {
            this.onRequest = onRequest;

            builder = Host.CreateDefaultBuilder().ConfigureWebHostDefaults(builder =>
            {
                builder.ConfigureServices(Services);

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

                builder.Configure(Setup);
            });
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

        private void Services(IServiceCollection services)
        {
            services.AddCors(cors =>
            {
                cors.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin();
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.SetPreflightMaxAge(TimeSpan.FromDays(1));
                    policy.Build();
                });
            });
        }

        private void Setup(IApplicationBuilder app)
        {
            app.UseCors();

            app.Run(async context =>
            {
                await onRequest.Invoke(context);
            });
        }
    }
}
