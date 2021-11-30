using Microsoft.AspNetCore.Http;

namespace JapeService.Responder
{
    public interface ITransfer
    {
        HttpRequest Request { get; }
        HttpResponse Response { get; }
    }
}
