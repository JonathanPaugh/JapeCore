using System.Threading.Tasks;
using JapeCore;
using JapeHttp;

namespace JapeDatabase
{
    public partial class DatabaseApi
    {
        private readonly Api api;

        public DatabaseApi(int port, string key)
        {
            api = new Api($"http://localhost:{port}", key);
        }

        private async Task<ApiResponse> Request(JsonData data)
        {
            ApiRequest request = api.Request(string.Empty);
            request.SetMethod(JapeHttp.Request.Method.Post);
            await request.WriteJsonAsync(new JsonData(data) {{ "key", api.Key }});
            return await request.GetResponseAsync();        
        }
    }
}
