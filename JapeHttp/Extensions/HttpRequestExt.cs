using System;
using System.IO;
using System.Threading.Tasks;
using JapeCore;
using Microsoft.AspNetCore.Http;

namespace JapeHttp
{
    public static class HttpRequestExt
    {
        public static string Read(this HttpRequest request, bool keepAlive = false)
        {
            using StreamReader reader = new(request.Body, null, true, -1, keepAlive);
            return reader.ReadToEnd();
        }

        public static JsonData ReadJson(this HttpRequest request, bool keepAlive = false)
        {
            string json = Read(request, keepAlive);
            return new JsonData(json);
        }

        public static async Task<string> ReadAsync(this HttpRequest request, bool keepAlive = false)
        {
            using StreamReader reader = new(request.Body, null, true, -1, keepAlive);
            return await reader.ReadToEndAsync();
        }

        public static async Task<JsonData> ReadJsonAsync(this HttpRequest request, bool keepAlive = false)
        {
            string json = await ReadAsync(request, keepAlive);
            return new JsonData(json);
        }
    }
}
