using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using JapeHttp;
using Microsoft.AspNetCore.Http;

namespace JapeDatabase
{
    public partial class Database
    {
        public async void Request(HttpContext context)
        {
            Log.Write("Database Request");

            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            response.SetCorsHeaders();

            if (request.Method == "OPTIONS")
            {
                await response.ResponseOptions();
                return;
            }

            Dictionary<string, JsonElement> data = await request.ReadJson();

            if (data == null)
            {
                Log.Write("Database Request Error: Empty Data");
                response.StatusCode = 401;
                await response.CompleteAsync();
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
                    await response.CompleteAsync();
                    break;
            }
        }

        public async void MongoRequest(HttpRequest request, HttpResponse response, Dictionary<string, JsonElement> data)
        {
            string id = data["id"].GetString();

            if (string.IsNullOrEmpty(id))
            {
                Log.Write("Database Request Error: Empty Id");
                response.StatusCode = 404;
                await response.CompleteAsync();
                return;
            }

            if (!mongoResponses.TryGetValue(id, out Action<HttpRequest, HttpResponse, Dictionary<string, JsonElement>> action))
            {
                Log.Write("Database Request Error: Incorrect Id");
                response.StatusCode = 404;
                await response.CompleteAsync();
                return;
            }

            action.Invoke(request, response, data);
        }

        public async void RedisRequest(HttpRequest request, HttpResponse response, Dictionary<string, JsonElement> data)
        {
            string id = data["id"].GetString();

            if (string.IsNullOrEmpty(id))
            {
                Log.Write("Database Request Error: Empty Id");
                response.StatusCode = 404;
                await response.CompleteAsync();
                return;
            }

            if (!redisResponses.TryGetValue(id, out Action<HttpRequest, HttpResponse, Dictionary<string, JsonElement>> action))
            {
                Log.Write("Database Request Error: Incorrect Id");
                response.StatusCode = 404;
                await response.CompleteAsync();
                return;
            }

            action.Invoke(request, response, data);
        }
    }
}
