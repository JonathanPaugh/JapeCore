using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using JapeHttp;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver.Core.Bindings;
using StackExchange.Redis;

namespace JapeDatabase
{
    public partial class Database
    {
        private Dictionary<string, Action<HttpRequest, HttpResponse, Dictionary<string, JsonElement>>> redisResponses;
        private void RedisResponses()
        {
            redisResponses = new Dictionary<string, Action<HttpRequest, HttpResponse, Dictionary<string, JsonElement>>>
            {
                { "Get", ResponseRedisGet },
                { "Set", ResponseRedisSet },
                { "Remove", ResponseRedisRemove },
                { "Subscribe", ResponseRedisSubscribe },
                { "Unsubscribe", ResponseRedisUnsubscribe },
                { "Publish", ResponseRedisPublish },
                { "Receive", ResponseRedisReceive },
            };
        }

        public async void ResponseRedisGet(HttpRequest request, HttpResponse response, Dictionary<string, JsonElement> data)
        {
            Log.Write("Get Request");

            IDatabase database = redis.GetDatabase();

            RedisValue value = await database.StringGetAsync(data["key"].GetString());

            response.StatusCode = 200;
            await response.Write(value.ToString());
            await response.CompleteAsync();
        }

        public async void ResponseRedisSet(HttpRequest request, HttpResponse response, Dictionary<string, JsonElement> data)
        {
            Log.Write("Set Request");

            IDatabase database = redis.GetDatabase();

            bool value = await database.StringSetAsync(data["key"].GetString(), data["value"].GetString());

            response.StatusCode = 200;
            await response.Write(value.ToString());
            await response.CompleteAsync();
        }

        public async void ResponseRedisRemove(HttpRequest request, HttpResponse response, Dictionary<string, JsonElement> data)
        {
            Log.Write("Remove Request");

            IDatabase database = redis.GetDatabase();

            bool value = await database.KeyDeleteAsync(data["key"].GetString());

            response.StatusCode = 200;
            await response.Write(value.ToString());
            await response.CompleteAsync();
        }

        private Dictionary<string, Redis.Subscription> subscriptions = new Dictionary<string, Redis.Subscription>();
        public async void ResponseRedisSubscribe(HttpRequest request, HttpResponse response, Dictionary<string, JsonElement> data)
        {
            Log.Write("Subscribe Request");

            ConnectionMultiplexer connection = redis.GetConnection();

            byte mode = data["mode"].GetByte();
            string channel = data["channel"].ToString();

            ISubscriber subscriber = connection.GetSubscriber();
            string key = GenerateSubscriptionKey();

            Redis.Subscription subscription = new Redis.Subscription(subscriber);
            subscriptions.Add(key, subscription);

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

            response.StatusCode = 200;
            await response.Write(key);
            await response.CompleteAsync();

            string GenerateSubscriptionKey()
            {
                string key;
                do 
                {
                    key = Guid.NewGuid().ToString();
                } while (subscriptions.ContainsKey(key));
                return key;
            }
        }

        public async void ResponseRedisUnsubscribe(HttpRequest request, HttpResponse response, Dictionary<string, JsonElement> data)
        {
            Log.Write("Unsubscribe Request");

            string key = data["subscription"].ToString();
            if (!subscriptions.TryGetValue(key, out Redis.Subscription subscription))
            {
                response.StatusCode = 404;
                await response.CompleteAsync();
                return;
            }

            subscription.Unsubscribe();
            subscriptions.Remove(key);

            string[] values = subscription.Pull();

            if (values.Length == 0)
            {
                response.StatusCode = 204;
                await response.CompleteAsync();
                return;
            }

            response.StatusCode = 200;
            await response.WriteJson(new Dictionary<string, object>
            {
                { "values", values }
            });
            await response.CompleteAsync();
        }

        public async void ResponseRedisPublish(HttpRequest request, HttpResponse response, Dictionary<string, JsonElement> data)
        {
            Log.Write("Publish Request");

            IDatabase database = redis.GetDatabase();

            long value = await database.PublishAsync(data["channel"].ToString(), data["value"].ToString());

            response.StatusCode = 200;
            await response.Write(value.ToString());
            await response.CompleteAsync();
        }

        public async void ResponseRedisReceive(HttpRequest request, HttpResponse response, Dictionary<string, JsonElement> data)
        {
            Log.Write("Receive Request");

            if (!subscriptions.TryGetValue(data["subscription"].ToString(), out Redis.Subscription subscription))
            {
                response.StatusCode = 404;
                await response.CompleteAsync();
                return;
            }

            string[] values = subscription.Pull();

            if (values.Length == 0)
            {
                response.StatusCode = 204;
                await response.CompleteAsync();
                return;
            }

            response.StatusCode = 200;
            await response.WriteJson(new Dictionary<string, object>
            {
                { "values", values }
            });
            await response.CompleteAsync();
        }
    }
}
