using System;
using System.Threading.Tasks;
using JapeService;
using JapeService.Responder;
using Microsoft.AspNetCore.Http;

namespace JapeDatabase
{
    public partial class Database : RestService
    {
        public const byte MongoIndex = 0;
        public const byte RedisIndex = 1;

        private Responder<byte> Director => GetResponder<byte>(DirectorName);
        private const string DirectorName = "Director";

        public Responder<string> MongoResponder => GetResponder<string>(MongoResponderName);
        private const string MongoResponderName = "Mongo";

        public Responder<string> RedisResponder => GetResponder<string>(RedisResponderName);
        private const string RedisResponderName = "Redis"; 

        private readonly bool useMongo;
        private readonly bool useRedis;

        protected Mongo mongo;
        protected Redis redis;

        public Database(int http, int https, bool useMongo, bool useRedis) : base(http, https) {
            this.useMongo = useMongo;
            this.useRedis = useRedis;
        }

        protected override async Task OnStartAsync()
        {
            if (useMongo)
            {
                mongo = Mongo.Connect(Mongo.Host, Mongo.Port, Mongo.User, Mongo.Password, Mongo.Database, Mongo.UseSsl, Mongo.ReplicaSet);
            }

            if (useRedis)
            {
                redis = Redis.Connect(Redis.Host, Redis.Port, Redis.User, Redis.Password, Redis.UseSsl);
            }

            await Task.CompletedTask;
        }

        protected override async Task OnRequest(HttpContext context)
        {
            await Director.Respond(context.Request, context.Response);
        }

        protected override ResponderList Responders(ResponderFactory factory)
        {
            IResponder mongoResponder = factory.Create(MongoResponderName, data => data.GetString("command").ToLowerInvariant()).Responses(MongoResponses).Build();
            IResponder redisResponder = factory.Create(RedisResponderName, data => data.GetString("command").ToLowerInvariant()).Responses(RedisResponses).Build();

            return new ResponderList
            {
                factory.Create(DirectorName, data => data.GetByte("index")).Responses(new ResponseBank<byte>
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
