using System;
using System.Threading.Tasks;
using JapeHttp;
using Microsoft.AspNetCore.Components.Routing;

namespace JapeService.Responder
{
    public partial class Responder<T>
    {
        internal class Intercepter
        {
            internal class Resolution : JapeHttp.Resolution
            {
                public bool Intercepted { get; }

                internal Resolution(bool intercepted)
                {
                    Intercepted = intercepted;
                }
            }
        }

        internal class RequestIntercepter : Intercepter
        {
            private readonly RequestInterception interception;

            internal RequestIntercepter(RequestInterception interception)
            {
                this.interception = interception;
            }

            internal async Task<JapeHttp.Resolution> Invoke(Intercept intercept, object[] args)
            {
                return await interception.Invoke(intercept, args);
            }
        }

        internal class ResponseIntercepter : Intercepter
        {
            private readonly ResponseInterception interception;

            internal ResponseIntercepter(ResponseInterception interception)
            {
                this.interception = interception;
            }

            internal async Task<JapeHttp.Resolution> Invoke(Intercept intercept, JsonData data, object[] args)
            {
                return await interception.Invoke(intercept, data, args);
            }
        }
    }
}