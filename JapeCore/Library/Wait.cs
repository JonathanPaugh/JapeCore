using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JapeCore.Library
{
    public static class Wait
    {
        public static async Task While(Func<bool> condition, TimeSpan interval, CancellationToken cancellationToken)
        {
            await Task.Run(Wait, cancellationToken);

            async Task Wait()
            {
                while (!cancellationToken.IsCancellationRequested && condition())
                {
                    await Task.Delay(interval, cancellationToken);
                }
            }
        }

        public static async Task While(Func<bool> condition, TimeSpan interval, TimeSpan timeout)
        {
            CancellationTokenSource cancellationSource = new();
            await While(condition, interval, cancellationSource.Token).Timeout(timeout, cancellationSource);
        }

        public static async Task Until(Func<bool> condition, TimeSpan interval, CancellationToken cancellationToken)
        {
            await Task.Run(Wait, cancellationToken);

            async Task Wait()
            {
                while (!cancellationToken.IsCancellationRequested && !condition())
                {
                    await Task.Delay(interval, cancellationToken);
                }
            }
        }

        public static async Task Until(Func<bool> condition, TimeSpan interval, TimeSpan timeout)
        {
            CancellationTokenSource cancellationSource = new();
            await Until(condition, interval, cancellationSource.Token).Timeout(timeout, cancellationSource);
        }
    }
}
