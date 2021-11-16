using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JapeCore;
using Microsoft.AspNetCore.Http;
using JapeHttp;

namespace JapeService
{
    public abstract class Service
    {
        protected string Name => $"{GetType().Name}";
        protected string StartString => $"{Name} Started";
        protected string StopString => $"{Name} Stopped";

        public static IEnumerable<ICommandArg> Args => new ICommandArg[]
        {
            CommandArg<int>.CreateOptional("--http", "<int> Starts an instance of the service at this http port"),
            CommandArg<int>.CreateOptional("--https", "<int> Starts an instance of the service at this https port")
        };

        protected readonly Listener listener;

        protected Service(int http, int https)
        {
            listener = new Listener(OnRequest);

            if (http > 0)
            {
                listener.CreateServer(http);
            }

            if (https > 0)
            {
                listener.CreateServerSecure(https);
            }
        }
        
        public void Start()
        {
            listener.Start();

            OnStartLow();

            Log.Write(StartString);

            Console.CancelKeyPress += OnCancelKey;
        }

        public void Stop()
        {
            OnStopLow();
            
            Log.Write(StopString);
        }

        private void OnCancelKey(object sender, ConsoleCancelEventArgs e)
        {
            Console.CancelKeyPress -= OnCancelKey;
            Stop();
        }

        internal virtual void OnStartLow() { OnStart(); }
        internal virtual void OnStopLow() { OnStart(); }

        protected virtual void OnStart() {}
        protected virtual void OnStop() {}

        protected abstract Task OnRequest(HttpContext context);
    }
}
