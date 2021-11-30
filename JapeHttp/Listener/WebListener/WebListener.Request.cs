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

            public override async Task<Resolution> Complete(Status.SuccessCode code)
            {
                await Close((int)code);
                return GetResolution();
            }

            public override async Task<Resolution> Complete(Status.SuccessCode code, string data)
            {
                await Close((int)code, data);
                return GetResolution();
            }

            public override async Task<Resolution> Complete(Status.SuccessCode code, JsonData data)
            {
                await Close((int)code, data);
                return GetResolution();
            }

            public override async Task<Resolution> Abort(Status.ErrorCode code)
            {
                await Close((int)code);
                return GetResolution();
            }
        }
    }

}
