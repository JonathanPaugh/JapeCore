using System;
using System.CommandLine.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace JapeCore
{
    public static class TaskExt
    {
        public static async Task Timeout(this Task task, TimeSpan timeout, CancellationTokenSource cancellationSource)
        {
            CancellationTokenSource timeoutSource = new();

            Task timeoutTask = Task.Delay(timeout, timeoutSource.Token);
            Task completedTask = await Task.WhenAny(task, timeoutTask);

            cancellationSource.Cancel();
            timeoutSource.Cancel();

            if (completedTask == timeoutTask)
            {
                throw new TimeoutException();
            } 
            
            await task;
        }
    }
}
