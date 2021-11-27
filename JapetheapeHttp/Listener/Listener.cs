using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
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

        private readonly IHostBuilder builder;

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

        public void SetLogLevel(LogLevel level) => builder.ConfigureLogging(logger => logger.SetMinimumLevel(level));

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

        public void Start()
        {
            Task.Run(() =>
            {
                IHost host = builder.UseConsoleLifetime().Build();
                host.Run();
                Environment.Exit(0);
            });
        }

        protected virtual void Services(IServiceCollection services) {}
        protected virtual void Setup(IApplicationBuilder app) {}
    }
}
