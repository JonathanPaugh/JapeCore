using System.Threading.Tasks;

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
    }
}