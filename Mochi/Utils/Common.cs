using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Mochi.Texts;

namespace Mochi.Utils;

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

    public static T BufferToStructure<T>(byte[] buf, int offset = 0)
    {
        var b = new byte[buf.Length - offset];
        Array.Copy(buf, offset, b, 0, b.Length);
        var ptr = Marshal.AllocHGlobal(b.Length);
        Marshal.Copy(b, 0, ptr, b.Length);
        var result = Marshal.PtrToStructure<T>(ptr);
        Marshal.FreeHGlobal(ptr);
        return result;
    }

    public static string? GetNullTerminatedWideString(IntPtr ptr)
    {
        if (ptr == IntPtr.Zero) return null;
        
        var charSize = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? 2 : 4;
        var ofs = 0;

        var characters = new List<int>();
        while (true)
        {
            var c = charSize == 2 ? Marshal.ReadInt16(ptr, ofs) : Marshal.ReadInt32(ptr, ofs);
            ofs += charSize;
            if (c == 0) break;
            characters.Add(c);
        }

        return new string(characters.Select(c => (char)c).ToArray());
    }
}