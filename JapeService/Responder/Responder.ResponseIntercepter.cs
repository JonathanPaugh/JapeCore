using System.Threading.Tasks;
using JapeHttp;

namespace JapeService.Responder
{
    public partial class Responder<T>
    {
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