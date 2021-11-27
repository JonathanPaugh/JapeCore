using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using JapeCore;
using JapeService;

namespace JapeDatabase
{
    internal class Program : ConsoleProgram<int, int>
    {
        protected override string DefaultLog => "database.log";

        protected override IEnumerable<ICommandArg> Args() => Service.Args;

        private int http;
        private int https;

        private static async Task Main(string[] args) => await Run<Program>(args);

        protected override void OnSetup(int http, int https)
        {
            this.http = http;
            this.https = https;
        }

        protected override void OnStart()
        {
            Database database = new(http, https);
            database.Start();
            WaitShutdown();
        }
    }
}
