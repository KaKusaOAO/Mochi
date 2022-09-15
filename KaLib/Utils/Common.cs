using System;
using System.Threading;
using System.Threading.Tasks;
using KaLib.Texts;

namespace KaLib.Utils;

public static class Common
{
    public static void DiscardAndCatch(Task task)
    {
        DiscardAndCatch(task, ex =>
        {
            Logger.Warn(LiteralText.Of($"Uncaught exception from task {task}:"));
            Logger.Warn(ex.ToString());
        });
    }
    
    public static void DiscardAndCatch(Task task, Action<Exception> handleException)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                handleException(ex);
            }
        });
    }

    public static void AcquireSemaphore(SemaphoreSlim semaphore, Action action)
    {
        semaphore.Wait();
        try
        {
            action();
        }
        finally
        {
            semaphore.Release();
        }
    }
    
    public static T AcquireSemaphore<T>(SemaphoreSlim semaphore, Func<T> action)
    {
        semaphore.Wait();
        try
        {
            return action();
        }
        finally
        {
            semaphore.Release();
        }
    }
    
    public static async Task AcquireSemaphoreAsync(SemaphoreSlim semaphore, Func<Task> action)
    {
        await semaphore.WaitAsync();
        try
        {
            await action();
        }
        finally
        {
            semaphore.Release();
        }
    }
    
    public static async Task AcquireSemaphoreAsync(SemaphoreSlim semaphore, Action action)
    {
        await semaphore.WaitAsync();
        try
        {
            action();
        }
        finally
        {
            semaphore.Release();
        }
    }
}