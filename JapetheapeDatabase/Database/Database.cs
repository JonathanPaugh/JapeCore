using System;
using JapeHttp;
using Microsoft.AspNetCore.Http;

namespace JapeDatabase
{
    public partial class Database
    {
        private const int Port = 1434;
        private const int PortSecure = 1443;

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

            Console.CancelKeyPress += delegate { OnShutdown(); };
        }

        public void OnShutdown()
        {
            Log.Write("Database Stopped");
        }

        private void StartListener()
        {
            Listener listener = new Listener(ListenerRequest);
            listener.CreateServer(Port);
            listener.CreateServerSecure(PortSecure);
            listener.Start();
        }

        private void ListenerRequest(HttpContext context)
        {
            Request(context);
        }
    }
}
