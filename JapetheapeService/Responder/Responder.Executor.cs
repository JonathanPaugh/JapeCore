using System;
using System.Threading.Tasks;
using JapeHttp;

namespace JapeService.Responder
{
    public partial class Responder<T>
    {
        private class Executor
        {
            private readonly Func<Transfer, JsonData, object[], Task<Resolution>> caller;

            public Executor(Func<Transfer, JsonData, object[], Task<Resolution>> caller)
            {
                this.caller = caller; 
            }

            internal async Task<Resolution> Invoke(Transfer transfer, JsonData data, object[] args)
            {
                return await caller.Invoke(transfer, data, args);
            }
        }
    }
}