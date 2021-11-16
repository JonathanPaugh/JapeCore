using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JapeHttp;
using Microsoft.AspNetCore.Http;

namespace JapeService.Responder
{
    public partial class Responder<T>
    {
        public abstract class Exchange
        {
            internal readonly HttpRequest request;
            internal readonly HttpResponse response;

            private Func<T, Transfer, JsonData, object[], Task<Resolution>> caller;

            public string Method => request.Method;

            internal Exchange(HttpRequest request, 
                              HttpResponse response, 
                              Func<T, Transfer, JsonData, object[], Task<Resolution>> caller)
            {
                this.request = request;
                this.response = response;
                this.caller = caller;
            }

            public abstract Task<Resolution> Complete(Status.SuccessCode code);
            public abstract Task<Resolution> Complete(Status.SuccessCode code, string data);
            public abstract Task<Resolution> Complete(Status.SuccessCode code, JsonData data);

            public abstract Task<Resolution> Abort(Status.ErrorCode code);

            protected async Task Close(int statusCode)
            {
                response.StatusCode = statusCode;
                await response.CompleteAsync();
            }

            protected async Task Close(int statusCode, string data)
            {
                response.StatusCode = statusCode;
                await response.Write(data);
                await response.CompleteAsync();
            }

            protected async Task Close(int statusCode, JsonData data)
            {
                response.StatusCode = statusCode;
                await response.WriteJson(data);
                await response.CompleteAsync();
            }

            protected async Task<Resolution> Invoke(T id, Transfer transfer, JsonData data, object[] args)
            {
                return await caller.Invoke(id, transfer, data, args);
            }
        }
    }
}