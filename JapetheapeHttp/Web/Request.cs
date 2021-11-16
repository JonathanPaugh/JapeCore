using System.IO;
using System.Net;

namespace JapeHttp
{
    public class Request
    {
        public enum Method { Get, Post, Create, Delete }

        private readonly WebRequest request;

        public Request(string url)
        {
            request = WebRequest.CreateHttp(url);
        }

        public Request SetMethod(Method method)
        {
            switch (method)
            {
                case Method.Get:
                    request.Method = "GET";
                    break;

                case Method.Post:
                    request.Method = "POST";
                    break;

                case Method.Create:
                    request.Method = "CREATE";
                    break;

                case Method.Delete:
                    request.Method = "DELETE";
                    break;
            }
            return this;
        }

        public Request SetContentType(string value)
        {
            request.ContentType = value;
            return this;
        }

        public Request AddHeader(string key, string value)
        {
            request.Headers.Add(key, value);
            return this;
        }

        public Request Write(string data)
        {
            using (StreamWriter writer = new(request.GetRequestStream()))
            {
                writer.Write(data);
            }
            return this;
        }

        public Request WriteJson(JsonData data)
        {
            return Write(data.Serialize());
        }

        public Response GetResponse()
        {
            return new Response(request);
        }
    }
}
