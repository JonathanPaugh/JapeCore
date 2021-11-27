using System.IO;
using System.Net;

namespace JapeHttp
{
    public class Response
    {
        private readonly string value;

        public Response(WebRequest request)
        {
            using WebResponse response = request.GetResponse();
            using StreamReader reader = new(response.GetResponseStream());
            value = reader.ReadToEnd();
        }

        public string Read() { return value; }
        public JsonData ReadJson() { return new JsonData(Read()); }
    }
}
