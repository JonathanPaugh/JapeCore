using System;
using System.IO;
using System.Threading.Tasks;
using JapeCore;
using Microsoft.AspNetCore.Http;

namespace JapeHttp
{
    public static class HttpRequestExt
    {
        public static string Read(this HttpRequest request, bool keepAlive = false) => request.Body.Read(keepAlive);
        public static JsonData ReadJson(this HttpRequest request, bool keepAlive = false) => request.Body.ReadJson(keepAlive);
        public static async Task<string> ReadAsync(this HttpRequest request, bool keepAlive = false) => await request.Body.ReadAsync(keepAlive);
        public static async Task<JsonData> ReadJsonAsync(this HttpRequest request, bool keepAlive = false) => await request.Body.ReadJsonAsync(keepAlive);
    }
}
