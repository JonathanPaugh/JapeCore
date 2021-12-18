using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

namespace JapeDatabase
{
    public partial class Mongo
    {
        public const ConnectionStringScheme Scheme = ConnectionStringScheme.MongoDB;

        public const string Host = "localhost";
        public const int Port = 27017;

        public const string User = "default";
        public const string Password = "3BBW8IJXuR6Ig8MDaOC6ARN4MQr9Tnpu4XBUwJdc9t3W8EIjWN7+YGkuefwB+hNBXpngUvdMQJ2/tj1H";

        public const string Database = "admin";

        public const string ReplicaSet = null;

        public const bool UseSsl = false;
        public const bool InsecureSsl = true;

        private static string CertificateFile => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mongo.crt");

        private readonly MongoClient connection;

        private Mongo(MongoClient connection)
        {
            this.connection = connection;
        }

        public static Mongo Connect(string connectionString)
        {
            MongoClientSettings settings = MongoClientSettings.FromConnectionString(connectionString);
            return new Mongo(new MongoClient(settings));
        }

        public static Mongo Connect(string host, int port, string user, string password, string database, bool useSSL, string replicaSet)
        {
            MongoClientSettings settings = new()
            {
                Scheme = Scheme,
                Server = new MongoServerAddress(host, port),
                Credential = MongoCredential.CreateCredential(database, user, password),
            };

            #pragma warning disable CS0162 // Unreachable code detected

            if (replicaSet != null)
            {
                settings.ReplicaSetName = ReplicaSet;
            }

            if (useSSL)
            {
                settings.UseTls = true;
                settings.AllowInsecureTls = InsecureSsl;

                X509Certificate2Collection certificates = new(new []
                {
                    new X509Certificate2(CertificateFile)
                });

                List<X509Certificate> temp = new();
                foreach (X509Certificate2 certificate in certificates)
                {
                    temp.Add(certificate);
                }

                settings.SslSettings = new SslSettings
                {
                    CheckCertificateRevocation = false,
                    ClientCertificates = temp,
                };

                temp.Clear();
            }

            #pragma warning restore CS0162 // Unreachable code detected

            return new Mongo(new MongoClient(settings));
        }

        public MongoClient GetConnection()
        {
            return connection;
        }

        public IMongoDatabase GetDatabase(string name)
        {
            return connection.GetDatabase(name);
        }
    }
}
