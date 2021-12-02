using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using JapeCore;
using JapeHttp;

namespace JapeService
{
    public abstract class Service
    {
        protected virtual string Name => $"{GetType().Name}";
        protected virtual string StartString => $"{Name} Started";
        protected virtual string StopString => $"{Name} Stopped";

        public static IEnumerable<ICommandArg> Args => new ICommandArg[]
        {
            CommandArg<int>.CreateOptional("--http", "<int> Starts an instance of the service at this http port"),
            CommandArg<int>.CreateOptional("--https", "<int> Starts an instance of the service at this https port")
        };

        private readonly Listener listener;

        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        protected Service(int http, int https)
        {
            listener = ServiceListener();

            if (http > 0)
            {
                listener.CreateServer(http);
            }

            if (https > 0)
            {
                listener.CreateServerSecure(https);
            }
        }

        public async Task Start()
        {
            Log.Write(StartString);

            listener.Construct();
            listener.Start();

            await OnStartLow();
        }

        public async Task Stop()
        {
            listener.Stop();

            Log.Write(StopString);

            await OnStopLow();
        }

        internal virtual async Task OnStartLow() => await Task.WhenAll(Task.Run(OnStart), OnStartAsync());
        internal virtual async Task OnStopLow() => await Task.WhenAll(Task.Run(OnStop), OnStopAsync());

        protected virtual void OnStart() {}
        protected virtual async Task OnStartAsync() => await Task.CompletedTask;

        protected virtual void OnStop() {}
        protected virtual async Task OnStopAsync() => await Task.CompletedTask;


        protected virtual Listener ServiceListener() => new();
    }
}
