using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JapeCore;
using JapeCore.Library;
using JapeService;

namespace JapeWeb
{
    internal class Program : ConsoleProgram<int, int>
    {
        protected override string DefaultLog => "server.log";

        private int http;
        private int https;

        private static async Task Main(string[] args) => await RunAsync<Program>(args);

        protected override IEnumerable<ICommandArg> Args() => Service.Args;

        protected override void OnSetup(int http, int https)
        {
            this.http = http;
            this.https = https;
        }

        protected override async Task OnStartAsync()
        {
            WebServer webServer = new(http, https);
            await webServer.Start();
        }
    }
}
