﻿using System;
using System.Threading.Tasks;
using JapeCore;
using JapeHttp;
using JapeService;
using JapeService.Responder;
using StackExchange.Redis;

namespace JapeDatabase
{
    public partial class Database
    {
        private ResponseBank<string> RedisResponses => new()
        {
            { "Get", ResponseRedisGet },
            { "Set", ResponseRedisSet },
            { "Remove", ResponseRedisRemove },
            { "Subscribe", ResponseRedisSubscribe },
            { "Unsubscribe", ResponseRedisUnsubscribe },
            { "Publish", ResponseRedisPublish },
            { "Receive", ResponseRedisReceive },
        };

        public async Task<Resolution> ResponseRedisGet(Responder<string>.Transfer transfer, JsonData data, object[] args)
        {
            Log.Write("Get Request");

            IDatabase database = redis.GetDatabase();

            RedisValue value = await database.StringGetAsync(data.GetString("key"));

            return await transfer.Complete(Status.SuccessCode.Ok, value.ToString());
        }

        public async Task<Resolution> ResponseRedisSet(Responder<string>.Transfer transfer, JsonData data, object[] args)
        {
            Log.Write("Set Request");

            IDatabase database = redis.GetDatabase();

            bool value = await database.StringSetAsync(data.GetString("key"), data.GetString("value"));

            return await transfer.Complete(Status.SuccessCode.Ok, value.ToString());
        }

        public async Task<Resolution> ResponseRedisRemove(Responder<string>.Transfer transfer, JsonData data, object[] args)
        {
            Log.Write("Remove Request");

            IDatabase database = redis.GetDatabase();

            bool value = await database.KeyDeleteAsync(data.GetString("key"));

            return await transfer.Complete(Status.SuccessCode.Ok, value.ToString());
        }

        public async Task<Resolution> ResponseRedisSubscribe(Responder<string>.Transfer transfer, JsonData data, object[] args)
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

        public async Task<Resolution> ResponseRedisUnsubscribe(Responder<string>.Transfer transfer, JsonData data, object[] args)
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

        public async Task<Resolution> ResponseRedisPublish(Responder<string>.Transfer transfer, JsonData data, object[] args)
        {
            Log.Write("Publish Request");

            IDatabase database = redis.GetDatabase();

            long value = await database.PublishAsync(data.GetString("channel"), data.GetString("value"));

            return await transfer.Complete(Status.SuccessCode.Ok, value.ToString());
        }

        public async Task<Resolution> ResponseRedisReceive(Responder<string>.Transfer transfer, JsonData data, object[] args)
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