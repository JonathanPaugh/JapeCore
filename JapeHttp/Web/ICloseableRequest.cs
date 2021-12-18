using System.Threading.Tasks;
using JapeCore;

namespace JapeHttp
{
    public interface ICloseableRequest<T> where T : Request.Result
    {
        Task<T> Complete(Status.SuccessCode code);
        Task<T> Complete(Status.SuccessCode code, string data);
        Task<T> Complete(Status.SuccessCode code, JsonData data);

        Task<T> Abort(Status.ErrorCode code);
    }
}
