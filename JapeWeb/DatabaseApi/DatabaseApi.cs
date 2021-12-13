using System;
using System.Threading.Tasks;
using JapeDatabase;
using JapeHttp;

namespace JapeWeb
{
    public partial class DatabaseApi : Database
    {
        private readonly Api api;

        public DatabaseApi(int port, string env) : base(port, 0, env, false, false)
        {
            api = new Api($"http://localhost:{port}");
        }

        protected override async Task OnStartAsync() => await Task.CompletedTask;

        public void UseMongo(string connectionString) => mongo = Mongo.Connect(connectionString);
        public void UseRedis(string connectionString) => redis = Redis.Connect(connectionString);

        private ApiResponse Request(JsonData data)
        {
            return api.Request(string.Empty)
                      .SetMethod(JapeHttp.Request.Method.Post)
                      .WriteJson(new JsonData(data) {{ "key", Key }})
                      .GetResponse();
        }
    }
}
