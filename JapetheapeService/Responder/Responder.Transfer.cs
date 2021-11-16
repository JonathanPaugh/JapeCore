using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JapeHttp;
using Microsoft.AspNetCore.Http;

namespace JapeService.Responder
{
    public interface ITransfer
    {
        HttpRequest Request { get; }
        HttpResponse Response { get; }
    }

    public partial class Responder<T>
    {
        public class Transfer : Exchange, ITransfer
        {
            public HttpRequest Request => request;
            public HttpResponse Response => response;

            internal Transfer(HttpRequest request, 
                              HttpResponse response, 
                              Func<T, Transfer, JsonData, object[], Task<Resolution>> caller) 
                              : base(request, response, caller) {}

            public override async Task<Resolution> Complete(Status.SuccessCode code)
            {
                await Close((int)code);
                return new Resolution();
            }

            public override async Task<Resolution> Complete(Status.SuccessCode code, string data)
            {
                await Close((int)code, data);
                return new Resolution();
            }

            public override async Task<Resolution> Complete(Status.SuccessCode code, JsonData data)
            {
                await Close((int)code, data);
                return new Resolution();
            }

            public override async Task<Resolution> Abort(Status.ErrorCode code)
            {
                await Close((int)code);
                return new Resolution();
            }

            public async Task<Resolution> Redirect(T id, Transfer transfer, JsonData data, object[] args)
            {
                return await Invoke(id, transfer, data, args);
            }
        }
    }
}