#nullable enable

using System;

namespace JapeCore
{
    public readonly struct CommandArg<T> : ICommandArg
    {
        Type ICommandArg.Type => typeof(T);

        bool ICommandArg.Optional => optional;
        string ICommandArg.Name => name;
        string ICommandArg.Description => description;
        Func<object?> ICommandArg.GetDefault => getDefault;
        string[] ICommandArg.Aliases => aliases;

        private readonly bool optional;
        private readonly string name;
        private readonly string description;
        private readonly Func<object?> getDefault;
        private readonly string[] aliases;

        private CommandArg(bool optional, string name, string description = null!, Func<object?>? getDefault = null, params string[] aliases)
        {
            this.optional = optional;
            this.name = name;
            this.description = description;
            this.getDefault = getDefault ?? getDefaultValue;
            this.aliases = aliases;
        }

        private static object? getDefaultValue() => default;

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
