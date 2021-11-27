using System.Collections.Generic;
using System.Threading.Tasks;
using JapeCore;
using JapeService;

namespace JapeWeb
{
    internal class Program : ConsoleProgram<int, int>
    {
        protected override string DefaultLog => "server.log";

        private int http;
        private int https;

        private static async Task Main(string[] args) => await Run<Program>(args);

        protected override IEnumerable<ICommandArg> Args() => Service.Args;

        protected override void OnSetup(int http, int https)
        {
            this.http = http;
            this.https = https;
        }

        protected override void OnStart()
        {
            WebServer webServer = new(http, https);
            webServer.Start();
            WaitShutdown();
        }
    }
}
