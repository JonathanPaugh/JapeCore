using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace JapeHttp
{
    public class ApiResponse
    {
        private readonly string data;

        private ApiResponse(string data) { this.data = data; } 
        
        public string Read() { return data; }
        public JsonData ReadJson() { return new JsonData(Read()); }

        internal static ApiResponse FromRequest(WebRequest request)
        {
            using WebResponse response = request.GetResponse();
            using StreamReader reader = new(response.GetResponseStream());

            string data = reader.ReadToEnd();

            return new ApiResponse(data);
        }

        internal static async Task<ApiResponse> FromRequestAsync(WebRequest request)
        {
            using WebResponse response = await request.GetResponseAsync();
            using StreamReader reader = new(response.GetResponseStream());

            string data = await reader.ReadToEndAsync();

            return new ApiResponse(data);
        }
    }
}
