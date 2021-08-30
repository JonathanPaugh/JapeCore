using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;

namespace JapeHttp
{
    public static class HttpListenerResponseExt
    {
        public static void WriteJson(this HttpListenerResponse response, Dictionary<string, object> data)
        {
            using (StreamWriter writer = new StreamWriter(response.OutputStream)) 
            {
                writer.Write(JsonSerializer.Serialize(data));
            }
        }

        public static void Write(this HttpListenerResponse response, string data)
        {
            using (StreamWriter writer = new StreamWriter(response.OutputStream))
            {
                writer.Write(data);
            }
        }

        public static void SetCorsHeaders(this HttpListenerResponse response)
        {
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Credentials", "true");
            response.Headers.Add("Access-Control-Max-Age", "86400");
        }

        public static void SetPreflightHeaders(this HttpListenerResponse response)
        {
            response.Headers.Add("Access-Control-Allow-Methods", "*");
            response.Headers.Add("Access-Control-Allow-Headers", "*");
        }

        public static void ResponseOptions(this HttpListenerResponse response)
        {
            Log.Write("Preflight Request");

            SetPreflightHeaders(response);

            response.StatusCode = 200;
            response.Close();
        }
    }
}
