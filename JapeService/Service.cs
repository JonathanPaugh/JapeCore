using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        protected Listener listener;

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

        internal virtual void OnStartLow() { OnStart(); }
        internal virtual void OnStopLow() { OnStop(); }

        protected virtual void OnStart() {}
        protected virtual void OnStop() {}

        protected virtual Listener ServiceListener() => new();

        private void OnCancelKey(object sender, ConsoleCancelEventArgs e)
        {
            Console.CancelKeyPress -= OnCancelKey;
            Stop();
        }
    }
}
