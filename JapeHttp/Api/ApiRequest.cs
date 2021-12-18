using System.IO;
using System.Net;
using System.Threading.Tasks;
using JapeCore;

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
            request.GetRequestStream().Write(data);
            return this;
        }

        public ApiRequest WriteJson(JsonData data)
        {
            request.GetRequestStream().WriteJson(data);
            return this;
        }

        public async Task<ApiRequest> WriteAsync(string data)
        {
            Stream stream = await request.GetRequestStreamAsync();
            await stream.WriteAsync(data);
            return this;
        }

        public async Task<ApiRequest> WriteJsonAsync(JsonData data)
        {
            Stream stream = await request.GetRequestStreamAsync();
            await stream.WriteJsonAsync(data);
            return this;
        }

        public ApiResponse GetResponse() => ApiResponse.FromRequest(request);
        public async Task<ApiResponse> GetResponseAsync() => await ApiResponse.FromRequestAsync(request);
    }
}
