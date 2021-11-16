#nullable enable

using System;

namespace JapeCore
{
    public readonly struct CommandArg<T> : ICommandArg
    {
        public Type Type => typeof(T);

        public bool Optional => optional;
        public string Name => name;
        public string Description => description;
        public Func<object?>? GetDefault => getDefault;
        public string[] Aliases => aliases;

        public readonly bool optional;
        public readonly string name;
        public readonly string description;
        public readonly Func<object?>? getDefault;
        public readonly string[] aliases;

        private CommandArg(bool optional, string name, string description = null!, Func<object?>? getDefault = null, params string[] aliases)
        {
            this.optional = optional;
            this.name = name;
            this.description = description;
            this.getDefault = getDefault;
            this.aliases = aliases;
        }

        public static CommandArg<T> CreateOptional(string name, string description = null!, Func<object?>? getDefault = null, params string[] aliases)
        {
            return new CommandArg<T>(true, name, description, getDefault, aliases);
        }

        public static CommandArg<T> CreateRequired(string name)
        {
            return new CommandArg<T>(false, name);
        }
    }
}
