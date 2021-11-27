#nullable enable

using System;

namespace JapeCore
{
    public interface ICommandArg
    {
        Type Type { get; }

        bool Optional { get; }
        string Name { get; }
        string Description { get; }
        Func<object?>? GetDefault { get; }
        string[] Aliases { get; }
    }
}
