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
    public static class HttpListenerResponseExt
    {
        public static async Task WriteJson(this HttpResponse response, Dictionary<string, object> data)
        {
            await using (StreamWriter writer = new StreamWriter(response.Body)) 
            {
                await writer.WriteAsync(JsonSerializer.Serialize(data));
            }
        }

        public static async Task Write(this HttpResponse response, string data)
        {
            await using (StreamWriter writer = new StreamWriter(response.Body))
            {
                await writer.WriteAsync(data);
            }
        }

        public static void SetCorsHeaders(this HttpResponse response)
        {
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Credentials", "true");
            response.Headers.Add("Access-Control-Max-Age", "86400");
        }

        public static void SetPreflightHeaders(this HttpResponse response)
        {
            response.Headers.Add("Access-Control-Allow-Methods", "*");
            response.Headers.Add("Access-Control-Allow-Headers", "*");
        }

        public static async Task ResponseOptions(this HttpResponse response)
        {
            Log.Write("Preflight Request");

            SetPreflightHeaders(response);

            response.StatusCode = 200;

            await response.CompleteAsync();
        }
    }
}
