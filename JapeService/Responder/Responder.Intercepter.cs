using System;
using System.Threading.Tasks;
using JapeHttp;

namespace JapeService.Responder
{
    public partial class Responder<T>
    {
        internal class Intercepter
        {
            internal class Resolution : Responder.Resolution
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
            private readonly Func<Intercept, object[], Task<Responder.Resolution>> onIntercept;

            internal RequestIntercepter(Func<Intercept, object[], Task<Responder.Resolution>> onIntercept)
            {
                this.onIntercept = onIntercept;
            }

            internal async Task<Responder.Resolution> Invoke(Intercept intercept, object[] args)
            {
                return await onIntercept.Invoke(intercept, args);
            }
        }

        internal class ResponseIntercepter : Intercepter
        {
            private readonly Func<Intercept, JsonData, object[], Task<Responder.Resolution>> onIntercept;

            internal ResponseIntercepter(Func<Intercept, JsonData, object[], Task<Responder.Resolution>> onIntercept)
            {
                this.onIntercept = onIntercept;
            }

            internal async Task<Responder.Resolution> Invoke(Intercept intercept, JsonData data, object[] args)
            {
                return await onIntercept.Invoke(intercept, data, args);
            }
        }
    }
}