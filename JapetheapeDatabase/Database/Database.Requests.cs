using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using JapeHttp;

namespace JapeDatabase
{
    public partial class Database
    {
        public void Request(HttpListenerRequest request, HttpListenerResponse response)
        {
            Log.Write("Database Request");

            Dictionary<string, JsonElement> data = Listener.ReadData(request);

            Listener.SetCorsHeaders(response);

            if (request.HttpMethod == "OPTIONS")
            {
                responses["Options"].Invoke(request, response, data);
                return;
            }

            if (data == null)
            {
                Log.Write("Database Request Error: Empty Data");
                response.StatusCode = 401;
                response.Close();
                return;
            }

            string id = data["id"].GetString();

            if (string.IsNullOrEmpty(id))
            {
                Log.Write("Database Request Error: Empty Id");
                response.StatusCode = 404;
                response.Close();
                return;
            }

            if (!responses.TryGetValue(id, out Action<HttpListenerRequest, HttpListenerResponse, Dictionary<string, JsonElement>> action))
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
