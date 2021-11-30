using System.IO;
using System.Net;

namespace JapeHttp
{
    public class ApiResponse
    {
        private readonly string value;

        public ApiResponse(WebRequest request)
        {
            using WebResponse response = request.GetResponse();
            using StreamReader reader = new(response.GetResponseStream());
            value = reader.ReadToEnd();
        }

        public string Read() { return value; }
        public JsonData ReadJson() { return new JsonData(Read()); }
    }
}
