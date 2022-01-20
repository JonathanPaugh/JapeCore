using System;
using System.Threading.Tasks;
using JapeCore;
using JapeHttp;
using JapeService.Responder;
using StackExchange.Redis;

namespace JapeDatabase
{
    public partial class Database
    {
        private ResponseBank<string> RedisResponses => new()
        {
            { "query", ResponseRedisQuery },
            { "get", ResponseRedisGet },
            { "set", ResponseRedisSet },
            { "remove", ResponseRedisRemove },
            { "subscribe", ResponseRedisSubscribe },
            { "unsubscribe", ResponseRedisUnsubscribe },
            { "publish", ResponseRedisPublish },
            { "receive", ResponseRedisReceive },
        };

        public async Task<Request.Result> ResponseRedisQuery(Responder<string>.Transfer transfer, JsonData data, object[] args)
        {
            Log.Write("Query Request");

            IDatabase database = redis.GetDatabase();

            RedisResult result = await database.ExecuteAsync(data.GetString("query"));

            return await transfer.Complete(Status.SuccessCode.Ok, result.ToString());
        }

        public async Task<Request.Result> ResponseRedisGet(Responder<string>.Transfer transfer, JsonData data, object[] args)
        {
            Log.Write("Get Request");

            IDatabase database = redis.GetDatabase();

            RedisValue value = await database.StringGetAsync(data.GetString("id"));

            return await transfer.Complete(Status.SuccessCode.Ok, value.ToString());
        }

        public async Task<Request.Result> ResponseRedisSet(Responder<string>.Transfer transfer, JsonData data, object[] args)
        {
            Log.Write("Set Request");

            IDatabase database = redis.GetDatabase();

            bool value = await database.StringSetAsync(data.GetString("id"), data.GetString("value"));

            return await transfer.Complete(Status.SuccessCode.Ok, value.ToString());
        }

        public async Task<Request.Result> ResponseRedisRemove(Responder<string>.Transfer transfer, JsonData data, object[] args)
        {
            Log.Write("Remove Request");

            IDatabase database = redis.GetDatabase();

            bool value = await database.KeyDeleteAsync(data.GetString("id"));

            return await transfer.Complete(Status.SuccessCode.Ok, value.ToString());
        }

        public async Task<Request.Result> ResponseRedisSubscribe(Responder<string>.Transfer transfer, JsonData data, object[] args)
        {
            Log.Write("Subscribe Request");

            ConnectionMultiplexer connection = redis.GetConnection();

            byte mode = data.GetByte("mode");
            string channel = data.GetString("channel");

            ISubscriber subscriber = connection.GetSubscriber();
            string key = GenerateSubscriptionKey();

            Redis.Subscription subscription = new(subscriber);
            redis.subscriptions.Add(key, subscription);

            switch (mode)
            {
                case 0:
                {
                    ChannelMessageQueue queue = await subscriber.SubscribeAsync(channel);
                    queue.OnMessage(message =>
                    {
                        subscription.Push(message.Message.ToString());
                    });
                    break;
                }

                case 1:
                {
                    await subscriber.SubscribeAsync(channel, (_, message) => {
                        subscription.Push(message.ToString());
                    });
                    break;
                }
            }

            return await transfer.Complete(Status.SuccessCode.Ok, key);

            string GenerateSubscriptionKey()
            {
                string tempKey;
                do 
                {
                    tempKey = Guid.NewGuid().ToString();
                } while (redis.subscriptions.ContainsKey(tempKey));
                return tempKey;
            }
        }

        public async Task<Request.Result> ResponseRedisUnsubscribe(Responder<string>.Transfer transfer, JsonData data, object[] args)
        {
            Log.Write("Unsubscribe Request");

            string key = data.GetString("subscription");
            if (!redis.subscriptions.TryGetValue(key, out Redis.Subscription subscription))
            {
                return await transfer.Abort(Status.ErrorCode.NotFound);
            }

            subscription.Unsubscribe();
            redis.subscriptions.Remove(key);

            string[] values = subscription.Pull();

            if (values.Length == 0)
            {
                return await transfer.Complete(Status.SuccessCode.Empty);
            }

            return await transfer.Complete(Status.SuccessCode.Ok, new JsonData
            {
                { "values", values }
            });
        }

        public async Task<Request.Result> ResponseRedisPublish(Responder<string>.Transfer transfer, JsonData data, object[] args)
        {
            Log.Write("Publish Request");

            IDatabase database = redis.GetDatabase();

            long value = await database.PublishAsync(data.GetString("channel"), data.GetString("value"));

            return await transfer.Complete(Status.SuccessCode.Ok, value.ToString());
        }

        public async Task<Request.Result> ResponseRedisReceive(Responder<string>.Transfer transfer, JsonData data, object[] args)
        {
            Log.Write("Receive Request");

            if (!redis.subscriptions.TryGetValue(data.GetString("subscription"), out Redis.Subscription subscription))
            {
                return await transfer.Abort(Status.ErrorCode.NotFound);
            }

            string[] values = subscription.Pull();

            if (values.Length == 0)
            {
                return await transfer.Abort(Status.ErrorCode.NotFound);
            }

            return await transfer.Complete(Status.SuccessCode.Ok, new JsonData
            {
                { "values", values }
            });
        }
    }
}
