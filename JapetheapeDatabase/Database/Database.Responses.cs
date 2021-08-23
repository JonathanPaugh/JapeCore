using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using JapeHttp;
using StackExchange.Redis;

namespace JapeDatabase
{
    public partial class Database
    {
        private void Responses()
        {
            responses = new Dictionary<string, Action<HttpListenerRequest, HttpListenerResponse, Dictionary<string, JsonElement>>>
            {
                { "Options", ResponseOptions },
                { "Set", ResponseSet },
                { "Get", ResponseGet },
            };
        }

        public void ResponseOptions(HttpListenerRequest request, HttpListenerResponse response, Dictionary<string, JsonElement> data)
        {
            Log.Write("Preflight Request");

            Listener.SetPreflightHeaders(response);

            response.StatusCode = 200;
            response.Close();
        }

        public void ResponseGet(HttpListenerRequest request, HttpListenerResponse response, Dictionary<string, JsonElement> data)
        {
            Log.Write("Get Request");

            IDatabase database = redis.GetDatabase();

            string value = database.StringGet(data["key"].GetString());

            Listener.WriteData(response, value);

            response.StatusCode = 200;
            response.Close();
        }

        public void ResponseSet(HttpListenerRequest request, HttpListenerResponse response, Dictionary<string, JsonElement> data)
        {
            Log.Write("Set Request");

            IDatabase database = redis.GetDatabase();

            database.StringSet(data["key"].GetString(), data["value"].GetString());

            response.StatusCode = 200;
            response.Close();
        }
    }
}
