using System;
using System.Threading.Tasks;
using JapeCore;
using JapeHttp;
using Microsoft.AspNetCore.Http;

namespace JapeService.Responder
{
    public partial class Responder<T>
    {
        public class Transfer : Exchange, ITransfer
        {
            public HttpRequest Request => request;
            public HttpResponse Response => response;

            internal Transfer(HttpRequest request, 
                              HttpResponse response, 
                              Caller caller) 
                              : base(request, response, caller) {}

            public async Task<Result> Complete(Status.SuccessCode code)
            {
                await Close((int)code);
                return GenerateResult();
            }

            public async Task<Result> Complete(Status.SuccessCode code, string data)
            {
                await Close((int)code, data);
                return GenerateResult();
            }

            public async Task<Result> Complete(Status.SuccessCode code, JsonData data)
            {
                await Close((int)code, data);
                return GenerateResult();
            }

            public async Task<Result> Abort(Status.ErrorCode code)
            {
                await Close((int)code);
                return GenerateResult();
            }

            public async Task<Result> Redirect(T id, Transfer transfer, JsonData data, object[] args)
            {
                return await Invoke(id, transfer, data, args);
            }
        }
    }
}