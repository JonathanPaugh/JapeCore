using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using JapeHttp;
using StackExchange.Redis;

namespace JapeDatabase
{
    public partial class Database
    {
        private const string Host = "dev-do-user-8817720-0.b.db.ondigitalocean.com";
        private const string Port = "25061";

        private const int ListenerPort = 6379;

        private Dictionary<string, Action<HttpListenerRequest, HttpListenerResponse, Dictionary<string, JsonElement>>> responses;

        private ConnectionMultiplexer redis;

        public void Start()
        {
            Connect();
            Responses();
            StartListener();
            Log.Write("Database Started");
        }

        private void StartListener()
        {
            Listener listener = new Listener(ListenerPort, ListenerRequest);
            listener.Start();
        }

        private void ListenerRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            response.KeepAlive = false;
            Request(request, response);
        }

        private void Connect()
        {
            ConfigurationOptions options = ConfigurationOptions.Parse($"{Host}:{Port}");
            options.User = "default";
            options.Password = "dznrqp8k5ywsgae3";
            options.AllowAdmin = true;
            options.Ssl = true;
            redis = ConnectionMultiplexer.Connect(options);
        }
    }
}
