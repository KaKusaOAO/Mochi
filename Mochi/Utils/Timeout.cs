using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mochi.Utils;

public class Timeout
{
    private enum TimeoutType
    {
        Interval, Timeout
    }

    private readonly CancellationTokenSource _cts = new();
    private readonly TimeoutType _type;
    private readonly int _timeout;
    private readonly Action _action;
        
    private Timeout(TimeoutType type, Action action, int timeout)
    {
        _type = type;
        _action = action;
        _timeout = Math.Max(0, timeout);
        Task.Run(RunAsync);
    }
        
    private async Task RunAsync()
    {
        while (!_cts.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_timeout, _cts.Token);
                if (_cts.IsCancellationRequested) break;
                _action();
            }
            catch (TaskCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Logger.Warn("Unhandled exception in timeout: " + ex);
            }
            
            if (_type == TimeoutType.Timeout) break;
        }
    }
        
    public void Cancel()
    {
        _cts.Cancel();
    }
        
    public static Timeout CreateInterval(Action action, int timeout)
    {
        return new Timeout(TimeoutType.Interval, action, timeout);
    }
        
    public static Timeout CreateInterval(Func<Task> action, int timeout)
    {
        async void Action()
        {
            await action();
        }

        return new Timeout(TimeoutType.Interval, Action, timeout);
    }
    
    public static Timeout CreateTimeout(Action action, int timeout)
    {
        return new Timeout(TimeoutType.Timeout, action, timeout);
    }
    
    public static Timeout CreateTimeout(Func<Task> action, int timeout)
    {
        async void Action()
        {
            await action();
        }

        return new Timeout(TimeoutType.Timeout, Action, timeout);
    }
}