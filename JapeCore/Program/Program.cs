using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JapeCore
{
    public abstract class Program
    {
        protected virtual string DefaultLog => "program.log";

        private bool exit;

        protected abstract IEnumerable<ICommandArg> Args();
        
        public void Shutdown() { exit = true; }
        public void WaitShutdown() { SpinWait.SpinUntil(() => exit); } 

        private static void SetArgs(Command command, IEnumerable<ICommandArg> commandArgs)
        {
            foreach (ICommandArg commandArg in commandArgs)
            {
                if (commandArg.Optional)
                {
                    command.AddOption(new Option(new[] { commandArg.Name }.Concat(commandArg.Aliases).ToArray(), 
                                                 commandArg.Description, 
                                                 commandArg.Type, 
                                                 commandArg.GetDefault));
                } 
                else
                {
                    command.AddArgument(new Argument(commandArg.Name));
                }
            }
        }

        private RootCommand CreateCommand(ICommandHandler handler)
        {
            RootCommand command = new();
            SetArgs(command, Args());
            command.Handler = handler;
            return command;
        }

        private async Task StartLow(string[] args, ICommandHandler handler)
        {
            Log.InitDefault(DefaultLog);
            RootCommand command = CreateCommand(handler);
            await command.InvokeAsync(args);
        }

        internal async Task Start(string[] args, Action onStart) => await StartLow(args, CommandHandler.Create(onStart));
        internal async Task Start<T1>(string[] args, Action<T1> onStart) => await StartLow(args, CommandHandler.Create(onStart));
        internal async Task Start<T1, T2>(string[] args, Action<T1, T2> onStart) => await StartLow(args, CommandHandler.Create(onStart));
        internal async Task Start<T1, T2, T3>(string[] args, Action<T1, T2, T3> onStart) => await StartLow(args, CommandHandler.Create(onStart));
        internal async Task Start<T1, T2, T3, T4>(string[] args, Action<T1, T2, T3, T4> onStart) => await StartLow(args, CommandHandler.Create(onStart));
        internal async Task Start<T1, T2, T3, T4, T5>(string[] args, Action<T1, T2, T3, T4, T5> onStart) => await StartLow(args, CommandHandler.Create(onStart));
        internal async Task Start<T1, T2, T3, T4, T5, T6>(string[] args, Action<T1, T2, T3, T4, T5, T6> onStart) => await StartLow(args, CommandHandler.Create(onStart));
        internal async Task Start<T1, T2, T3, T4, T5, T6, T7>(string[] args, Action<T1, T2, T3, T4, T5, T6, T7> onStart) => await StartLow(args, CommandHandler.Create(onStart));
        internal async Task Start<T1, T2, T3, T4, T5, T6, T7, T8>(string[] args, Action<T1, T2, T3, T4, T5, T6, T7, T8> onStart) => await StartLow(args, CommandHandler.Create(onStart));
    }
}
