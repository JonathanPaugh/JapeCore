using System.IO;
using System.Net;

namespace JapeHttp
{
    public class ApiRequest
    {
        private readonly WebRequest request;

        public ApiRequest(string url)
        {
            request = WebRequest.CreateHttp(url);
        }

        public ApiRequest SetMethod(Request.Method method)
        {
            switch (method)
            {
                case Request.Method.Get:
                    request.Method = "GET";
                    break;

                case Request.Method.Post:
                    request.Method = "POST";
                    break;

                case Request.Method.Create:
                    request.Method = "CREATE";
                    break;

                case Request.Method.Delete:
                    request.Method = "DELETE";
                    break;
            }
            return this;
        }

        public ApiRequest SetContentType(string value)
        {
            request.ContentType = value;
            return this;
        }

        public ApiRequest AddHeader(string key, string value)
        {
            request.Headers.Add(key, value);
            return this;
        }

        public ApiRequest Write(string data)
        {
            using (StreamWriter writer = new(request.GetRequestStream()))
            {
                writer.Write(data);
            }
            return this;
        }

        public ApiRequest WriteJson(JsonData data)
        {
            return Write(data.Serialize());
        }

        public ApiResponse GetResponse()
        {
            return new ApiResponse(request);
        }
    }
}
