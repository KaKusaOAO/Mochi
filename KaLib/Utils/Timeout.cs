using System;
using System.Threading;
using System.Threading.Tasks;

namespace KaLib.Utils
{
    public class Timeout
    {
        private enum TimeoutType
        {
            Interval, Timeout
        }

        private CancellationTokenSource cts = new();
        private TimeoutType type;
        private int timeout;
        private Action action;
        
        private Timeout(TimeoutType type, Action action, int timeout)
        {
            this.type = type;
            this.action = action;
            this.timeout = Math.Max(0, timeout);
            Task.Run(RunAsync);
        }
        
        private async Task RunAsync()
        {
            while (!cts.IsCancellationRequested)
            {
                await Task.Delay(timeout);
                if (cts.IsCancellationRequested) break;
                action();
                if (type == TimeoutType.Timeout) break;
            }
        }
        
        public void Cancel()
        {
            cts.Cancel();
        }
        
        public static Timeout Interval(Action action, int timeout)
        {
            return new Timeout(TimeoutType.Interval, action, timeout);
        }
        
        public static Timeout Interval(Func<Task> action, int timeout)
        {
            async void Action()
            {
                await action();
            }

            return new Timeout(TimeoutType.Interval, Action, timeout);
        }
    }
}