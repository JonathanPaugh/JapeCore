using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace JapeHttp
{
    public class RequestListener : Listener
    {
        private readonly Func<HttpContext, Task> onRequest;

        public RequestListener(Func<HttpContext, Task> onRequest)
        {
            this.onRequest = onRequest;
        }

        protected sealed override void Services(IServiceCollection services)
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

        protected sealed override void Setup(IApplicationBuilder app)
        {
            app.UseCors();

            app.Run(async context =>
            {
                await onRequest.Invoke(context);
            });
        }
    }
}
