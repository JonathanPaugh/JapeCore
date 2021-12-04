using System;
using System.Threading.Tasks;
using JapeHttp;

namespace JapeService.Responder
{
    public partial class Responder<T>
    {
        private class Executor
        {
            private readonly Response response;

            public Executor(Response response)
            {
                this.response = response; 
            }

            internal async Task<Request.Result> Invoke(Transfer transfer, JsonData data, object[] args)
            {
                return await response.Invoke(transfer, data, args);
            }
        }
    }
}