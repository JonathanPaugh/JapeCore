using System;
using System.Threading.Tasks;
using JapeService;
using JapeService.Responder;
using Microsoft.AspNetCore.Http;

namespace JapeDatabase
{
    public partial class Database : RestService
    {
        private Responder<byte> Director => GetResponder<byte>(DirectorName);
        private const string DirectorName = "Director";

        public Responder<string> MongoResponder => GetResponder<string>(MongoResponderName);
        private const string MongoResponderName = "Mongo";

        public Responder<string> RedisResponder => GetResponder<string>(RedisResponderName);
        private const string RedisResponderName = "Redis";

        public const byte MongoIndex = 0;
        public const byte RedisIndex = 1;

        private Mongo mongo;
        private Redis redis;

        private readonly bool useMongo;
        private readonly bool useRedis;

        public Database(int http, int https, bool useMongo, bool useRedis) : base(http, https) {
            this.useMongo = useMongo;
            this.useRedis = useRedis;
        }

        public void UseMongo(string host, int port, string user, string password, string database, bool useSsl = false, string replicaSet = null)
        {
            mongo = Mongo.Connect(host, port, user, password, database, useSsl, replicaSet);
        }

        public void UseMongo(string connectionString)
        {
            mongo = Mongo.Connect(connectionString);
        }

        public void UseRedis(string host, int port, string user, string password, bool useSsl = false)
        {
            redis = Redis.Connect(host, port, user, password, useSsl);
        }

        protected override void OnStart()
        {
            if (useMongo && mongo == null)
            {
                UseMongo(Mongo.Host, Mongo.Port, Mongo.User, Mongo.Password, Mongo.Database, Mongo.UseSsl, Mongo.ReplicaSet);
            }

            if (useRedis && redis == null)
            {
                UseRedis(Redis.Host, Redis.Port, Redis.User, Redis.Password, Redis.UseSsl);
            }
        }

        protected override async Task OnRequest(HttpContext context)
        {
            await Director.Respond(context.Request, context.Response);
        }

        protected override ResponderList Responders(ResponderFactory factory)
        {
            IResponder mongoResponder = factory.Create(MongoResponderName, data => data.GetString("id").ToLowerInvariant()).Responses(MongoResponses).Build();
            IResponder redisResponder = factory.Create(RedisResponderName, data => data.GetString("id").ToLowerInvariant()).Responses(RedisResponses).Build();

            return new ResponderList
            {
                factory.Create("Director", data => data.GetByte("index")).Responses(new ResponseBank<byte>
                {
                    { 0, mongoResponder.Invoke },
                    { 1, redisResponder.Invoke }
                }).Build(),

                mongoResponder,
                redisResponder
            };
        }
    }
}
