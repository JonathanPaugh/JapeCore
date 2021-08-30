using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using JapeHttp;
using MongoDB.Bson;
using MongoDB.Driver;
using StackExchange.Redis;

namespace JapeDatabase
{
    public partial class Database
    {
        private const int Port = 1434;

        private Mongo mongo;
        private Redis redis;

        public void Start()
        {
            mongo = Mongo.Connect();
            MongoResponses();

            redis = Redis.Connect();
            RedisResponses();

            StartListener();

            Log.Write("Database Started");
        }

        private void StartListener()
        {
            Listener listener = new Listener(Port, ListenerRequest);
            listener.Start();
        }

        private void ListenerRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            response.KeepAlive = false;
            Request(request, response);
        }
    }
}
