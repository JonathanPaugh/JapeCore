using System;
using System.Threading;
using System.Threading.Tasks;

namespace JapeCore
{
    public static class TaskFactoryExt
    {
        public static async Task Routine(this TaskFactory factory, Action action, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await factory.StartNew(action, cancellationToken);
            }
        }

        public static async Task Routine(this TaskFactory factory, Func<Task> function, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await factory.StartNew(function, cancellationToken).Unwrap();
            }
        }

        public static async Task Routine(this TaskFactory factory, Action action, TimeSpan timeout)
        {
            CancellationTokenSource cancellationSource = new();
            await Routine(factory, action, cancellationSource.Token).Timeout(timeout, cancellationSource);
        }

        public static async Task Routine(this TaskFactory factory, Func<Task> function, TimeSpan timeout)
        {
            CancellationTokenSource cancellationSource = new();
            await Routine(factory, function, cancellationSource.Token).Timeout(timeout, cancellationSource);
        }
    }
}
