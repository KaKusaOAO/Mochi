﻿using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Mochi.Utils;

public class TaskQueue
{
    private ConcurrentQueue<Func<Task>> Queue { get; } = new();

    private readonly SemaphoreSlim _loopLock = new(1, 1);

    public int Count => Queue.Count;
        
    public bool IsActive { get; private set; }

    public void Enqueue(Func<Task> task)
    {
        var doStart = Queue.IsEmpty;
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
        await _loopLock.WaitAsync();
        IsActive = true;
        while (!Queue.IsEmpty)
        {
            if (!Queue.TryDequeue(out var task)) continue;
            await task().ConfigureAwait(false);
        }
        IsActive = false;
        _loopLock.Release();
    }
}