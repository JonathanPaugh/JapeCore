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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        public Responder<string> MongoResponder => GetResponder<string>(MongoResponderName);
        private const string MongoResponderName = "Mongo";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        public Responder<string> RedisResponder => GetResponder<string>(RedisResponderName);
        private const string RedisResponderName = "Redis";

        private Mongo mongo;
        private Redis redis;

        public Database(int http, int https) : base(http, https) {}

        protected override void OnStart()
        {
            mongo = Mongo.Connect();
            redis = Redis.Connect();
        }

        protected override async Task OnRequest(HttpContext context)
        {
            await Director.Respond(context.Request, context.Response);
        }

        protected override ResponderList Responders(ResponderFactory factory)
        {
            IResponder mongoResponder = factory.Create("Mongo", data => data.GetString("id")).Responses(MongoResponses).Build();
            IResponder redisResponder = factory.Create("Redis", data => data.GetString("id")).Responses(RedisResponses).Build();

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
