using System;
using System.Threading.Tasks;
using JapeHttp;
using Microsoft.AspNetCore.Http;

namespace JapeService.Responder
{
    public partial class Responder<T>
    {
        public class Intercept : Exchange
        {
            internal Intercept(HttpRequest request, 
                               HttpResponse response, 
                               Caller caller) 
                               : base(request, response, caller) {}

            public override async Task<Result> Complete(Status.SuccessCode code)
            {
                await Close((int)code);
                return new Intercepter.Result(true);
            }

            public override async Task<Result> Complete(Status.SuccessCode code, string data)
            {
                await Close((int)code, data);
                return new Intercepter.Result(true);
            }

            public override async Task<Result> Complete(Status.SuccessCode code, JsonData data)
            {
                await Close((int)code, data);
                return new Intercepter.Result(true);
            }

            public override async Task<Result> Abort(Status.ErrorCode code)
            {
                await Close((int)code);
                return new Intercepter.Result(true);
            }

            public async Task<Result> Redirect(T id, Intercept intercept, JsonData data, object[] args)
            {
                await Invoke(id, new Transfer(intercept.request, intercept.response, intercept.Invoke), data, args);
                return new Intercepter.Result(true);
            }

            public Result Pass()
            {
                return new Intercepter.Result(false);
            }
        }
    }
}