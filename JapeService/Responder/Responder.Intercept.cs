using System;
using System.Threading.Tasks;
using JapeHttp;
using Microsoft.AspNetCore.Http;

namespace JapeService.Responder
{
    public partial class Responder<T>
    {
        public class Intercept : Exchange, ICloseableRequest<Intercept.Result>
        {
            internal Intercept(HttpRequest request, 
                               HttpResponse response, 
                               Caller caller) 
                               : base(request, response, caller) {}


            public async Task<Result> Complete(Status.SuccessCode code)
            {
                await Close((int)code);
                return new Result(true);
            }

            public async Task<Result> Complete(Status.SuccessCode code, string data)
            {
                await Close((int)code, data);
                return new Result(true);
            }

            public async Task<Result> Complete(Status.SuccessCode code, JsonData data)
            {
                await Close((int)code, data);
                return new Result(true);
            }

            public async Task<Result> Abort(Status.ErrorCode code)
            {
                await Close((int)code);
                return new Result(true);
            }

            public async Task<Result> Redirect(T id, Intercept intercept, JsonData data, object[] args)
            {
                await Invoke(id, new Transfer(intercept.request, intercept.response, intercept.Invoke), data, args);
                return new Result(true);
            }

            public Result Pass()
            {
                return new Result(false);
            }

            public new class Result : Request.Result
            {
                public bool Intercepted { get; }

                internal Result(bool intercepted)
                {
                    Intercepted = intercepted;
                }
            }
        }
    }
}