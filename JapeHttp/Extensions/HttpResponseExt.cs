using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace JapeHttp
{
    public static class HttpResponseExt
    {
        public static void Write(this HttpResponse response, string data, bool keepAlive = false)
        {
            using StreamWriter writer = new(response.Body, null, -1, keepAlive);
            writer.Write(data);
        }

        public static void WriteJson(this HttpResponse response, JsonData data, bool keepAlive = false)
        {
            Write(response, data.Serialize(), keepAlive);
        }

        public static async Task WriteAsync(this HttpResponse response, string data, bool keepAlive = false)
        {
            await using StreamWriter writer = new(response.Body, null, -1, keepAlive);
            await writer.WriteAsync(data);
        }

        public static async Task WriteJsonAsync(this HttpResponse response, JsonData data, bool keepAlive = false)
        {
            await WriteAsync(response, data.Serialize(), keepAlive);
        }

        public static bool IsCached(this HttpResponse response)
        {
            return !response.Headers.GetCommaSeparatedValues(HeaderNames.CacheControl).Contains(Request.NoCache);
        }
    }
}
