using System.IO;
using System.Net;
using System.Threading.Tasks;
using JapeCore;

namespace JapeHttp
{
    public class ApiRequest
    {
        private readonly WebRequest request;

        internal ApiRequest(string url)
        {
            request = WebRequest.CreateHttp(url);
        }

        public void SetMethod(Request.Method method)
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
        }

        public void SetContentType(string value)
        {
            request.ContentType = value;
        }

        public void AddHeader(string key, string value)
        {
            request.Headers.Add(key, value);
        }

        public void Write(string data)
        {
            request.GetRequestStream().Write(data);
        }

        public void WriteJson(JsonData data)
        {
            request.GetRequestStream().WriteJson(data);
        }

        public async Task WriteAsync(string data)
        {
            Stream stream = await request.GetRequestStreamAsync();
            await stream.WriteAsync(data);
        }

        public async Task WriteJsonAsync(JsonData data)
        {
            Stream stream = await request.GetRequestStreamAsync();
            await stream.WriteJsonAsync(data);
        }

        public ApiResponse GetResponse() => ApiResponse.FromRequest(request);
        public async Task<ApiResponse> GetResponseAsync() => await ApiResponse.FromRequestAsync(request);
    }
}
