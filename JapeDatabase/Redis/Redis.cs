using System.Collections.Generic;
using StackExchange.Redis;

namespace JapeDatabase
{
    public class Redis
    {
        private const string Host = "localhost";
        private const int Port = 6379;

        private const string User = "default";
        private const string Password = "F9jXYBrqX4inwUPclA9HezWEb/YYCOjl8D7obBrZYV62Vx5SO90K3z+PpkbW8Z1hbOAOyF+fCyJXbACQ";

        private const bool UseSSL = false;

        private readonly ConnectionMultiplexer connection;

        public readonly Dictionary<string, Subscription> subscriptions = new();

        private Redis(ConnectionMultiplexer connection)
        {
            this.connection = connection;
        }

        public static Redis Connect()
        {
            ConfigurationOptions settings = ConfigurationOptions.Parse($"{Host}:{Port}");

            settings.AllowAdmin = true;

            if (User != null)
            {
                settings.User = User;
            }

            settings.Password = Password;

            settings.Ssl = UseSSL;

            return new Redis(ConnectionMultiplexer.Connect(settings));
        }

        public ConnectionMultiplexer GetConnection()
        {
            return connection;
        }

        public IDatabase GetDatabase()
        {
            return connection.GetDatabase();
        }

        public class Subscription
        {
            public readonly ISubscriber subscriber;
            private readonly List<string> values = new();

            public Subscription(ISubscriber subscriber)
            {
                this.subscriber = subscriber;
            }

            public async void Unsubscribe()
            {
                await subscriber.UnsubscribeAllAsync();
            }

            public void Push(RedisValue value)
            {
                values.Add(value);
            }

            public string[] Pull()
            {
                string[] temp = values.ToArray();
                values.Clear();
                return temp;
            }
        }
    }
}
