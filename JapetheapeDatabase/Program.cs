using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using JapeCore;
using JapeService;

namespace JapeDatabase
{
    public class Program
    {
        private static Args? args;

        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        private static void Main(string[] args)
        {
            Log.Init("database");

            RootCommand commandRoot = new();

            AddArgs(commandRoot, Service.Args);

            commandRoot.Handler = CommandHandler.Create<int, int>(SetArgs);

            commandRoot.Invoke(args);
            
            SpinWait.SpinUntil(() => Program.args != null);

            Database database = new(Program.args.Value.http, 
                                    Program.args.Value.https);

            database.Start();

            SpinWait.SpinUntil(() => false);
        }

        private static void SetArgs(int http, int https)
        {
            args = new Args(http, https);
        }

        private static void AddArgs(Command command, IEnumerable<ICommandArg> commandArgs)
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

        private readonly struct Args
        {
            public readonly int http;
            public readonly int https;

            public Args(int http, int https)
            {
                this.http = http;
                this.https = https;
            }
        }
    }
}
