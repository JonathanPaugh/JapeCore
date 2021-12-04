using System;
using System.Threading.Tasks;
using JapeHttp;
using Microsoft.AspNetCore.Components.Routing;

namespace JapeService.Responder
{
    public partial class Responder<T>
    {
        internal class RequestIntercepter
        {
            private readonly RequestInterception interception;

            internal RequestIntercepter(RequestInterception interception)
            {
                this.interception = interception;
            }

            internal async Task<Intercept.Result> Invoke(Intercept intercept, object[] args)
            {
                return await interception.Invoke(intercept, args);
            }
        }

        internal class ResponseIntercepter
        {
            private readonly ResponseInterception interception;

            internal ResponseIntercepter(ResponseInterception interception)
            {
                this.interception = interception;
            }

            internal async Task<Intercept.Result> Invoke(Intercept intercept, JsonData data, object[] args)
            {
                return await interception.Invoke(intercept, data, args);
            }
        }
    }
}