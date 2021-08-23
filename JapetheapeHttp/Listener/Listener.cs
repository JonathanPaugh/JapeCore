using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace JapeHttp
{
    public class Listener
    {
        private HttpListener listener;
        private Action<HttpListenerRequest, HttpListenerResponse> onRequest;

        public Listener(int port, Action<HttpListenerRequest, HttpListenerResponse> onRequest)
        {
            listener = new HttpListener();
            listener.Prefixes.Add($"http://+:{port}/");
            this.onRequest = onRequest;
        }

        public void Start()
        {
            listener.Start();
            listener.BeginGetContext(Process, listener);
        }

        private void Process(IAsyncResult result)
        {
            HttpListenerContext context = ((HttpListener)result.AsyncState).EndGetContext(result);
            listener.BeginGetContext(Process, listener);

            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            onRequest(request, response);
        }

        public static Dictionary<string, JsonElement> ReadData(HttpListenerRequest request)
        {
            using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding)) 
            {
                string data = reader.ReadToEnd();
                if (string.IsNullOrEmpty(data)) { return null; }
                return JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(data);
            }
        }

        public static void WriteData(HttpListenerResponse response, Dictionary<string, object> data)
        {
            using (StreamWriter writer = new StreamWriter(response.OutputStream)) 
            {
                writer.Write(JsonSerializer.Serialize(data));
            }
        }

        public static void WriteData(HttpListenerResponse response, string data)
        {
            using (StreamWriter writer = new StreamWriter(response.OutputStream))
            {
                writer.Write(data);
            }
        }

        public static void SetCorsHeaders(HttpListenerResponse response)
        {
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Credentials", "true");
            response.Headers.Add("Access-Control-Max-Age", "86400");
        }

        public static void SetPreflightHeaders(HttpListenerResponse response)
        {
            response.Headers.Add("Access-Control-Allow-Methods", "*");
            response.Headers.Add("Access-Control-Allow-Headers", "*");
        }
    }
}
