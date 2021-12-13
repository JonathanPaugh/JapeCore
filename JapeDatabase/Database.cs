using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JapeCore;
using JapeHttp;
using JapeService;
using JapeService.Responder;
using Microsoft.AspNetCore.Http;

namespace JapeDatabase
{
    public partial class Database : RestService
    {
        private const string DefaultEnv = "API_JAPE_DATABASE";

        public const byte MongoIndex = 0;
        public const byte RedisIndex = 1;

        private Responder<byte> Director => GetResponder<byte>(DirectorName);
        private const string DirectorName = "Director";

        public Responder<string> MongoResponder => GetResponder<string>(MongoResponderName);
        private const string MongoResponderName = "Mongo";

        public Responder<string> RedisResponder => GetResponder<string>(RedisResponderName);
        private const string RedisResponderName = "Redis"; 

        public new static IEnumerable<ICommandArg> Args => new ICommandArg[]
        {
            CommandArg<string>.CreateOptional("--env", "Environment variable for key", () => DefaultEnv),
        };

        protected string Key => Environment.GetEnvironmentVariable(env);

        private readonly string env;

        private readonly bool useMongo;
        private readonly bool useRedis;

        protected Mongo mongo;
        protected Redis redis;

        public Database(int http, int https, string env, bool useMongo, bool useRedis) : base(http, https) {
            this.env = env;
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
                }).InterceptResponse(async (intercept, data, _) =>
                {
                    string key = data.GetString("key");

                    if (Key != key)
                    {
                        Log.Write("Database Request Error: Invalid Key");
                        Log.Write(key);
                        return await intercept.Abort(Status.ErrorCode.Forbidden);
                    }

                    return intercept.Pass();
                }).Build(),

                mongoResponder,
                redisResponder
            };
        }
    }
}
