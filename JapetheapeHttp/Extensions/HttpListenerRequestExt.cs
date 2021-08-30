using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;

namespace JapeHttp
{
    public static class HttpListenerRequestExt
    {
        public static string Read(this HttpListenerRequest request)
        {
            using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding)) 
            {
                string data = reader.ReadToEnd();
                return data;
            }
        }

        public static Dictionary<string, JsonElement> ReadJson(this HttpListenerRequest request)
        {
            using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding)) 
            {
                string data = reader.ReadToEnd();
                if (string.IsNullOrEmpty(data)) { return null; }
                return JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(data);
            }
        }
    }
}
