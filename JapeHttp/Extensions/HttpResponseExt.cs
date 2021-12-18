using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JapeCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace JapeHttp
{
    public static class HttpResponseExt
    {
        public static void Write(this HttpResponse response, string data, bool keepAlive = false) => response.Body.Write(data, keepAlive);
        public static void WriteJson(this HttpResponse response, JsonData data, bool keepAlive = false) => response.Body.WriteJson(data, keepAlive);
        public static async Task WriteAsync(this HttpResponse response, string data, bool keepAlive = false) => await response.Body.WriteAsync(data, keepAlive);
        public static async Task WriteJsonAsync(this HttpResponse response, JsonData data, bool keepAlive = false) => await response.Body.WriteJsonAsync(data, keepAlive);

        public static bool IsCached(this HttpResponse response) => !response.Headers.GetCommaSeparatedValues(HeaderNames.CacheControl)
                                                                                    .Contains(Request.NoCache);
    }
}
