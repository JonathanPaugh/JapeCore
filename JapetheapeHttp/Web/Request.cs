using System;
using System.IO;
using System.Net;
using System.Text;

namespace JapeHttp
{
    public class Request
    {
        public enum Method { Get, Post, Create, Delete }

        private readonly HttpWebRequest request;

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

        public Request Write(object value)
        {
            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(value);
            }
            return this;
        }

        public Response GetResponse()
        {
            return new Response(request);
        }
    }
}
