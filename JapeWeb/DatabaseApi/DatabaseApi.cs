using System;
using System.Threading.Tasks;
using JapeDatabase;
using JapeHttp;

namespace JapeWeb
{
    public partial class DatabaseApi
    {
        private const byte MongoIndex = 0;

        private readonly Api api;

        public DatabaseApi(int port, string key)
        {
            api = new Api($"http://localhost:{port}", key);
        }

        private async Task<ApiResponse> Request(JsonData data)
        {
            return await api.Request(string.Empty)
                            .SetMethod(JapeHttp.Request.Method.Post)
                            .WriteJson(new JsonData(data) {{ "key", api.Key }})
                            .GetResponseAsync();
        }
    }
}
