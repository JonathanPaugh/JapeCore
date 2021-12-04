using System.Collections.Generic;
using StackExchange.Redis;

namespace JapeDatabase
{
    public class Redis
    {
        internal const string Host = "localhost";
        internal const int Port = 6379;

        internal const string User = "default";
        internal const string Password = "F9jXYBrqX4inwUPclA9HezWEb/YYCOjl8D7obBrZYV62Vx5SO90K3z+PpkbW8Z1hbOAOyF+fCyJXbACQ";

        internal const bool UseSsl = false;

        internal readonly Dictionary<string, Subscription> subscriptions = new();

        private readonly ConnectionMultiplexer connection;

        private Redis(ConnectionMultiplexer connection)
        {
            this.connection = connection;
        }

        public static Redis Connect(string connectionString)
        {
            return new Redis(ConnectionMultiplexer.Connect(connectionString));
        }

        public static Redis Connect(string host, int port, string user, string password, bool useSsl)
        {
            ConfigurationOptions settings = ConfigurationOptions.Parse($"{host}:{port}");

            settings.AllowAdmin = true;

            if (user != null)
            {
                settings.User = user;
            }

            settings.Password = password;

            settings.Ssl = useSsl;

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
