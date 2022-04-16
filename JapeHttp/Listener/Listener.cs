using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using JapeCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JapeHttp
{
    public class Listener
    {
        private static string CertificateFile => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../ssl.pfx");

        public bool Running { get; private set; }
        public bool Contructed => host != null;

        private readonly IHostBuilder builder;
        private IHost host;

        public Listener()
        {
            builder = Host.CreateDefaultBuilder().ConfigureWebHostDefaults(builder =>
            {
                builder.ConfigureServices(Services);

                builder.ConfigureLogging(logger =>
                {
                    logger.SetMinimumLevel(LogLevel.Warning);
                });

                builder.UseKestrel(kestrel =>
                {
                    kestrel.AllowSynchronousIO = true;
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

        public void SetLogLevel(LogLevel level)
        {
            if (PreventIfConstructed(nameof(SetLogLevel))) { return; }

            builder.ConfigureLogging(logger => logger.SetMinimumLevel(level));
        }

        public void CreateServer(int port)
        {
            if (PreventIfConstructed(nameof(CreateServer))) { return; }

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
            if (PreventIfConstructed(nameof(CreateServerSecure))) { return; }

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

        public void Construct()
        {
            if (PreventIfConstructed(nameof(Construct))) { return; }

            host = builder.UseConsoleLifetime().Build();
        }

        public void Start()
        {
            if (PreventIfUnconstructed(nameof(Start))) { return; }
            if (PreventIfRunning(nameof(Start))) { return; }

            #pragma warning disable CS4014
            Run();
            #pragma warning restore CS4014
        }

        public void Stop()
        {
            if (PreventIfUnconstructed(nameof(Stop))) { return; }
            if (PreventIfIdle(nameof(Stop))) { return; }

            host.StopAsync();
        }

        private async Task Run()
        {
            Running = true;
            await host.RunAsync();
            Running = false;
        }

        protected virtual void Services(IServiceCollection services) {}
        protected virtual void Setup(IApplicationBuilder app) {}

        protected bool PreventIfConstructed(string name) => Prevention("constructed", name, () => Contructed);
        protected bool PreventIfUnconstructed(string name) => Prevention("unconstructed", name, () => !Contructed);
        protected bool PreventIfRunning(string name) => Prevention("running", name, () => Running);
        protected bool PreventIfIdle(string name) => Prevention("idle", name, () => !Running);

        private static bool Prevention(string message, string name, Func<bool> condition)
        {
            if (condition())
            {
                Log.Write($"Unable to execute action while listener is {message}: {name}");
                return true;
            }

            return false;
        }
    }
}
