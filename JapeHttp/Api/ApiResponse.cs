using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace JapeHttp
{
    public class ApiResponse
    {
        public string Data { get; }

        private JsonData json;
        public JsonData Json
        {
            get
            {
                if (Data == null) { return null; }
                if (json != null) { return json; }
                json = new JsonData(Data);
                return json;
            }
        }

        private ApiResponse(string data) { Data = data; } 

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
