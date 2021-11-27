using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using JapeHttp;

namespace JapeService
{
    public abstract class RequestService : Service
    {
        protected RequestService(int http, int https) : base(http, https) {}

        protected override Listener ServiceListener() => new RequestListener(OnRequest);

        public void Receive(HttpContext context) => OnRequest(context);

        protected abstract Task OnRequest(HttpContext context);
    }
}
