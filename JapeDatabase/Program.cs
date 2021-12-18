using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JapeCore;
using JapeService;

namespace JapeDatabase
{
    internal class Program : ConsoleProgram<int, int, string>
    {
        protected override string DefaultLog => "database.log";

        protected override IEnumerable<ICommandArg> Args() => Database.Args.Concat(Service.Args);

        private int http;
        private int https;
        private string env;

        private static async Task Main(string[] args) => await RunAsync<Program>(args);

        protected override void OnSetup(int http, int https, string env)
        {
            this.http = http;
            this.https = https;
            this.env = env;
        }

        protected override async Task OnStartAsync()
        {
            Database database = new(http, https, Environment.GetEnvironmentVariable(env));
            database.UseMongo(Mongo.Host, Mongo.Port, Mongo.User, Mongo.Password, Mongo.Database, Mongo.UseSsl, Mongo.ReplicaSet);
            database.UseRedis(Redis.Host, Redis.Port, Redis.User, Redis.Password, Redis.UseSsl);

            await database.Start();
        }
    }
}
