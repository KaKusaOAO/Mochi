using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace KaLib.Utils
{
    public class TaskQueue
    {
        private ConcurrentQueue<Func<Task>> Queue { get; } = new();

        public void Enqueue(Func<Task> task)
        {
            if (task == null)
            {
                Logger.Warn("Are you queueing a null task?");
                return;
            }

            bool doStart = Queue.Count == 0;
            Queue.Enqueue(task);
            if (doStart)
            {
                Task.Run(async () =>
                {
                    await InternalLoop().ConfigureAwait(false);
                });
            }
        }
        
        private async Task InternalLoop()
        {
            while (Queue.Count > 0)
            {
                if (!Queue.TryDequeue(out var task))
                {
                    continue;
                }
                await task().ConfigureAwait(false);
            }
        }
    }
}