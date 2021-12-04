using System.Threading.Tasks;
using JapeHttp;
using Microsoft.AspNetCore.Http;

namespace JapeHttp
{
    public partial class WebListener
    {
        public class Request : JapeHttp.Request
        {
            internal Request(HttpRequest request, HttpResponse response) : base(request, response) {}

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
        }
    }

}
