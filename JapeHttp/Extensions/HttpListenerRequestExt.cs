using System;
using System.IO;
using System.Threading.Tasks;
using JapeCore;
using Microsoft.AspNetCore.Http;

namespace JapeHttp
{
    public static class HttpRequestExt
    {
        public static async Task<string> Read(this HttpRequest request)
        {
            using StreamReader reader = new(request.Body);
            return await reader.ReadToEndAsync();
        }

        public static async Task<string> ReadStream(this HttpRequest request)
        {
            using StreamReader reader = new(request.Body, null, true, -1, true);
            return await reader.ReadToEndAsync();
        }

        public static async Task<JsonData> ReadJson(this HttpRequest request)
        {
            string json = await Read(request);
            return new JsonData(json);
        }

        public static async Task<JsonData> ReadJsonStream(this HttpRequest request)
        {
            string json = await ReadStream(request);
            return new JsonData(json);
        }
    }
}
