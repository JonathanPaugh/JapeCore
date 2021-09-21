using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace JapeHttp
{
    public static class HttpListenerRequestExt
    {
        public static async Task<string> Read(this HttpRequest request)
        {
            using (StreamReader reader = new StreamReader(request.Body)) 
            {
                return await reader.ReadToEndAsync();
            }
        }

        public static async Task<Dictionary<string, JsonElement>> ReadJson(this HttpRequest request)
        {
            using (StreamReader reader = new StreamReader(request.Body)) 
            {
                string data = await reader.ReadToEndAsync();
                if (string.IsNullOrEmpty(data)) { return null; }
                return JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(data);
            }
        }
    }
}
