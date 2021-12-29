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
        private const string ResponseIndexer = "command";

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
            CommandArg<string>.CreateOptional("--mongo", "Environment variable for key"),
            CommandArg<string>.CreateOptional("--redis", "Environment variable for key"),
        };

        private readonly string key;

        protected Mongo mongo;
        protected Redis redis;

        public Database(int http, int https, string key) : base(http, https) {
            this.key = key;
        }

        public void UseMongo(string connectionString) => mongo = Mongo.Connect(connectionString);
        public void UseMongo(string host, int port, string user, string password, string database, bool useSSL, string replicaSet) => mongo = Mongo.Connect(host, 
                                                                                                                                                            port, 
                                                                                                                                                            user, 
                                                                                                                                                            password, 
                                                                                                                                                            database, 
                                                                                                                                                            useSSL, 
                                                                                                                                                            replicaSet);

        public void UseRedis(string connectionString) => redis = Redis.Connect(connectionString);
        public void UseRedis(string host, int port, string user, string password, bool useSsl) => redis = Redis.Connect(host,
                                                                                                                        port,
                                                                                                                        user,
                                                                                                                        password,
                                                                                                                        useSsl);

        protected override async Task OnRequest(HttpContext context)
        {
            await Director.Respond(context.Request, context.Response);
        }

        protected override ResponderList Responders(ResponderFactory factory)
        {
            IResponder mongoResponder = factory.Create(MongoResponderName, data => data.GetString(ResponseIndexer).ToLowerInvariant())
                                               .Responses(MongoResponses)
                                               .Build();

            IResponder redisResponder = factory.Create(RedisResponderName, data => data.GetString(ResponseIndexer).ToLowerInvariant())
                                               .Responses(RedisResponses)
                                               .Build();

            IResponder director = factory.Create(DirectorName, data => data.GetByte("index")).Responses(new ResponseBank<byte>
            {
                { MongoIndex, mongoResponder.Invoke },
                { RedisIndex, redisResponder.Invoke }
            }).InterceptResponse(async (intercept, data, _) =>
            {
                string key = data.GetString("key");

                if (this.key != key)
                {
                    Log.Write("Database Request Error: Invalid Key");
                    return await intercept.Abort(Status.ErrorCode.Forbidden);
                }

                return intercept.Pass();
            }).Build();

            return new ResponderList
            {
                director,
                mongoResponder,
                redisResponder
            };
        }
    }
}
