using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using JapeHttp;

namespace JapeDatabase
{
    public partial class Database
    {
        public void Request(HttpListenerRequest request, HttpListenerResponse response)
        {
            Log.Write("Database Request");

            Dictionary<string, JsonElement> data = request.ReadJson();

            response.SetCorsHeaders();

            if (request.HttpMethod == "OPTIONS")
            {
                response.ResponseOptions();
                return;
            }

            if (data == null)
            {
                Log.Write("Database Request Error: Empty Data");
                response.StatusCode = 401;
                response.Close();
                return;
            }

            byte index = data["index"].GetByte();

            switch (index)
            {
                case 0:
                    MongoRequest(request, response, data);
                    break;

                case 1:
                    RedisRequest(request, response, data);
                    break;

                default:
                    Log.Write("Database Request Error: Invalid Index");
                    response.StatusCode = 404;
                    response.Close();
                    break;
            }
        }

        public void MongoRequest(HttpListenerRequest request, HttpListenerResponse response, Dictionary<string, JsonElement> data)
        {
            string id = data["id"].GetString();

            if (string.IsNullOrEmpty(id))
            {
                Log.Write("Database Request Error: Empty Id");
                response.StatusCode = 404;
                response.Close();
                return;
            }

            if (!mongoResponses.TryGetValue(id, out Action<HttpListenerRequest, HttpListenerResponse, Dictionary<string, JsonElement>> action))
            {
                Log.Write("Database Request Error: Incorrect Id");
                response.StatusCode = 404;
                response.Close();
                return;
            }

            action.Invoke(request, response, data);
        }

        public void RedisRequest(HttpListenerRequest request, HttpListenerResponse response, Dictionary<string, JsonElement> data)
        {
            string id = data["id"].GetString();

            if (string.IsNullOrEmpty(id))
            {
                Log.Write("Database Request Error: Empty Id");
                response.StatusCode = 404;
                response.Close();
                return;
            }

            if (!redisResponses.TryGetValue(id, out Action<HttpListenerRequest, HttpListenerResponse, Dictionary<string, JsonElement>> action))
            {
                Log.Write("Database Request Error: Incorrect Id");
                response.StatusCode = 404;
                response.Close();
                return;
            }

            action.Invoke(request, response, data);
        }
    }
}
