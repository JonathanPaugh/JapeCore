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
        private const string Host = "localhost";
        private const int Port = 27017;
        
        private const ConnectionStringScheme Scheme = ConnectionStringScheme.MongoDB;

        private const string Database = "admin";
        private const string User = "default";
        private const string Password = "3BBW8IJXuR6Ig8MDaOC6ARN4MQr9Tnpu4XBUwJdc9t3W8EIjWN7+YGkuefwB+hNBXpngUvdMQJ2/tj1H";

        private const string ReplicaSet = null;

        private const bool UseSSL = false;
        private const bool InsecureSSL = true;

        private static string CertificateFile => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mongo.crt");

        private static MongoClient connection;

        private Mongo() {} 
        
        public static Mongo Connect()
        {
            Mongo mongo = new();

            MongoClientSettings settings = new()
            {
                Scheme = Scheme,
                Server = new MongoServerAddress(Host, Port),
                Credential = MongoCredential.CreateCredential(Database, User, Password),
            };

            if (ReplicaSet != null)
            {
                settings.ReplicaSetName = ReplicaSet;
            }

            if (UseSSL)
            {
                settings.UseTls = true;
                settings.AllowInsecureTls = InsecureSSL;

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

            connection = new MongoClient(settings);

            return mongo;
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
