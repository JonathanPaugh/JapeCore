using System;
using System.Threading;
using System.Threading.Tasks;

namespace JapeCore
{
    public static class TaskFactoryExt
    {
        public static async Task StartWithTimeout(this TaskFactory factory, Action action, TimeSpan timeout)
        {
            await StartWithTimeoutLow(factory, 
                                      action, 
                                      timeoutSource => timeoutSource,
                                      timeout);
        }

        public static async Task StartWithTimeout(this TaskFactory factory, Action action, CancellationToken cancellationToken, TimeSpan timeout)
        {
            await StartWithTimeoutLow(factory, 
                                      action, 
                                      timeoutSource => CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutSource.Token), 
                                      timeout);
        }

        public static async Task<TResult> StartWithTimeout<TResult>(this TaskFactory<TResult> factory, Func<TResult> function, TimeSpan timeout)
        {
            return await StartWithTimeoutLow(factory, 
                                             function, 
                                             timeoutSource => timeoutSource,
                                             timeout);
        }

        public static async Task<TResult> StartWithTimeout<TResult>(this TaskFactory<TResult> factory, Func<TResult> function, CancellationToken cancellationToken, TimeSpan timeout)
        {
            return await StartWithTimeoutLow(factory, 
                                             function, 
                                             timeoutSource => CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutSource.Token), 
                                             timeout);
        }

        private static async Task StartWithTimeoutLow(this TaskFactory factory, Action action, Func<CancellationTokenSource, CancellationTokenSource> getTokenSource, TimeSpan timeout)
        {
            CancellationTokenSource timeoutSource = new();
            CancellationTokenSource tokenSource = getTokenSource(timeoutSource);

            Task task = factory.StartNew(action, tokenSource.Token);
            Task timeoutTask = Task.Delay(timeout, timeoutSource.Token);

            Task completedTask = await Task.WhenAny(task, timeoutTask);

            timeoutSource.Cancel();

            if (completedTask == timeoutTask) { throw new TimeoutException(); }

            await task;
        }

        private static async Task<TResult> StartWithTimeoutLow<TResult>(this TaskFactory<TResult> factory, Func<TResult> function, Func<CancellationTokenSource, CancellationTokenSource> getTokenSource, TimeSpan timeout)
        {
            CancellationTokenSource timeoutSource = new();
            CancellationTokenSource tokenSource = getTokenSource(timeoutSource);

            Task<TResult> task = factory.StartNew(function, tokenSource.Token);
            Task timeoutTask = Task.Delay(timeout, timeoutSource.Token);

            Task completedTask = await Task.WhenAny(task, timeoutTask);

            timeoutSource.Cancel();

            if (completedTask == timeoutTask) { throw new TimeoutException(); }

            return await task;
        }
    }
}
