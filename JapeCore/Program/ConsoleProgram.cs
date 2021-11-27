using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace JapeCore
{
    public abstract class ConsoleProgram : Program
    {
        protected abstract void OnSetup();

        protected virtual void OnStart(string[] args) {}
        protected virtual Task OnStartAsync(string[] args) { return Task.CompletedTask; }

        [SuppressMessage("ReSharper", "MethodHasAsyncOverload")]
        public static async Task Run<TProgram>(string[] args) where TProgram : ConsoleProgram, new()
        {
            TProgram program = new();
            await program.Start(args, program.OnSetup);
            program.OnStart(args);
        } 

        public static async Task RunAsync<TProgram>(string[] args) where TProgram : ConsoleProgram, new()
        {
            TProgram program = new();
            await program.Start(args, program.OnSetup);
            await program.OnStartAsync(args);
        } 
    }

    public abstract class ConsoleProgram<T1> : Program
    {
        protected abstract void OnSetup(T1 arg1);

        protected virtual void OnStart() {}
        protected virtual Task OnStartAsync() { return Task.CompletedTask; }

        [SuppressMessage("ReSharper", "MethodHasAsyncOverload")]
        public static async Task Run<TProgram>(string[] args) where TProgram : ConsoleProgram<T1>, new()
        {
            TProgram program = new();
            await program.Start<T1>(args, program.OnSetup);
            program.OnStart();
        }

        public static async Task RunAsync<TProgram>(string[] args) where TProgram : ConsoleProgram<T1>, new()
        {
            TProgram program = new();
            await program.Start<T1>(args, program.OnSetup);
            await program.OnStartAsync();
        }
    }

    public abstract class ConsoleProgram<T1, T2> : Program
    {
        protected abstract void OnSetup(T1 arg1, T2 arg2);

        protected virtual void OnStart() {}
        protected virtual Task OnStartAsync() { return Task.CompletedTask; }

        [SuppressMessage("ReSharper", "MethodHasAsyncOverload")]
        public static async Task Run<TProgram>(string[] args) where TProgram : ConsoleProgram<T1, T2>, new()
        {
            TProgram program = new();
            await program.Start<T1, T2>(args, program.OnSetup);
            program.OnStart();
        }

        public static async Task RunAsync<TProgram>(string[] args) where TProgram : ConsoleProgram<T1, T2>, new()
        {
            TProgram program = new();
            await program.Start<T1, T2>(args, program.OnSetup);
            await program.OnStartAsync();
        }
    }

    public abstract class ConsoleProgram<T1, T2, T3> : Program
    {
        protected abstract void OnSetup(T1 arg1, T2 arg2, T3 arg3);

        protected virtual void OnStart() {}
        protected virtual Task OnStartAsync() { return Task.CompletedTask; }

        [SuppressMessage("ReSharper", "MethodHasAsyncOverload")]
        public static async Task Run<TProgram>(string[] args) where TProgram : ConsoleProgram<T1, T2, T3>, new()
        {
            TProgram program = new();
            await program.Start<T1, T2, T3>(args, program.OnSetup);
            program.OnStart();
        } 

        public static async Task RunAsync<TProgram>(string[] args) where TProgram : ConsoleProgram<T1, T2, T3>, new()
        {
            TProgram program = new();
            await program.Start<T1, T2, T3>(args, program.OnSetup);
            await program.OnStartAsync();
        } 
    }

    public abstract class ConsoleProgram<T1, T2, T3, T4> : Program
    {
        protected abstract void OnSetup(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

        protected virtual void OnStart() {}
        protected virtual Task OnStartAsync() { return Task.CompletedTask; }

        [SuppressMessage("ReSharper", "MethodHasAsyncOverload")]
        public static async Task Run<TProgram>(string[] args) where TProgram : ConsoleProgram<T1, T2, T3, T4>, new()
        {
            TProgram program = new();
            await program.Start<T1, T2, T3, T4>(args, program.OnSetup);
            program.OnStart();
        } 

        public static async Task RunAsync<TProgram>(string[] args) where TProgram : ConsoleProgram<T1, T2, T3, T4>, new()
        {
            TProgram program = new();
            await program.Start<T1, T2, T3, T4>(args, program.OnSetup);
            await program.OnStartAsync();
        } 
    }

    public abstract class ConsoleProgram<T1, T2, T3, T4, T5> : Program
    {
        protected abstract void OnSetup(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

        protected virtual void OnStart() {}
        protected virtual Task OnStartAsync() { return Task.CompletedTask; }

        [SuppressMessage("ReSharper", "MethodHasAsyncOverload")]
        public static async Task Run<TProgram>(string[] args) where TProgram : ConsoleProgram<T1, T2, T3, T4, T5>, new()
        {
            TProgram program = new();
            await program.Start<T1, T2, T3, T4, T5>(args, program.OnSetup);
            program.OnStart();
        } 

        public static async Task RunAsync<TProgram>(string[] args) where TProgram : ConsoleProgram<T1, T2, T3, T4, T5>, new()
        {
            TProgram program = new();
            await program.Start<T1, T2, T3, T4, T5>(args, program.OnSetup);
            await program.OnStartAsync();
        } 
    }

    public abstract class ConsoleProgram<T1, T2, T3, T4, T5, T6> : Program
    {
        protected abstract void OnSetup(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

        protected virtual void OnStart() {}
        protected virtual Task OnStartAsync() { return Task.CompletedTask; }

        [SuppressMessage("ReSharper", "MethodHasAsyncOverload")]
        public static async Task Run<TProgram>(string[] args) where TProgram : ConsoleProgram<T1, T2, T3, T4, T5, T6>, new()
        {
            TProgram program = new();
            await program.Start<T1, T2, T3, T4, T5, T6>(args, program.OnSetup);
            program.OnStart();
        }

        public static async Task RunAsync<TProgram>(string[] args) where TProgram : ConsoleProgram<T1, T2, T3, T4, T5, T6>, new()
        {
            TProgram program = new();
            await program.Start<T1, T2, T3, T4, T5, T6>(args, program.OnSetup);
            await program.OnStartAsync();
        } 
    }

    public abstract class ConsoleProgram<T1, T2, T3, T4, T5, T6, T7> : Program
    {
        protected abstract void OnSetup(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);

        protected virtual void OnStart() {}
        protected virtual Task OnStartAsync() { return Task.CompletedTask; }

        [SuppressMessage("ReSharper", "MethodHasAsyncOverload")]
        public static async Task Run<TProgram>(string[] args) where TProgram : ConsoleProgram<T1, T2, T3, T4, T5, T6, T7>, new()
        {
            TProgram program = new();
            await program.Start<T1, T2, T3, T4, T5, T6, T7>(args, program.OnSetup);
            program.OnStart();
        }

        public static async Task RunAsync<TProgram>(string[] args) where TProgram : ConsoleProgram<T1, T2, T3, T4, T5, T6, T7>, new()
        {
            TProgram program = new();
            await program.Start<T1, T2, T3, T4, T5, T6, T7>(args, program.OnSetup);
            await program.OnStartAsync();
        } 
    }

    public abstract class ConsoleProgram<T1, T2, T3, T4, T5, T6, T7, T8> : Program
    {
        protected abstract void OnSetup(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

        protected virtual void OnStart() {}
        protected virtual Task OnStartAsync() { return Task.CompletedTask; }

        [SuppressMessage("ReSharper", "MethodHasAsyncOverload")]
        public static async Task Run<TProgram>(string[] args) where TProgram : ConsoleProgram<T1, T2, T3, T4, T5, T6, T7, T8>, new()
        {
            TProgram program = new();
            await program.Start<T1, T2, T3, T4, T5, T6, T7, T8>(args, program.OnSetup);
            program.OnStart();
        } 

        public static async Task RunAsync<TProgram>(string[] args) where TProgram : ConsoleProgram<T1, T2, T3, T4, T5, T6, T7, T8>, new()
        {
            TProgram program = new();
            await program.Start<T1, T2, T3, T4, T5, T6, T7, T8>(args, program.OnSetup);
            await program.OnStartAsync();
        } 
    }
}
