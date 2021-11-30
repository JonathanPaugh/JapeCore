using System;
using System.Threading.Tasks;
using JapeHttp;
using Microsoft.AspNetCore.Http;

namespace JapeService.Responder
{
    public partial class Responder<T>
    {
        public abstract class Exchange : Request
        {
            private readonly Caller caller;

            public delegate Task<Resolution> Caller(T id, Transfer transfer, JsonData data, object[] args);

            internal Exchange(HttpRequest request, HttpResponse response, Caller caller) : base(request, response)
            {
                this.caller = caller;
            }

            protected async Task<Resolution> Invoke(T id, Transfer transfer, JsonData data, object[] args)
            {
                return await caller.Invoke(id, transfer, data, args);
            }
        }
    }
}