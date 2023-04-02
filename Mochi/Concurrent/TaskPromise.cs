using System;
using System.Threading.Tasks;
using Mochi.Core;

namespace Mochi.Concurrent;

public abstract class TaskPromise : IPromise
{
    protected bool HasRejectHandler { get; set; }
    protected TaskPromise SourcePromise { get; set; }
    public abstract Task AsTask();
    
    protected void SetHasRejectHandler()
    {
        HasRejectHandler = true;
        SourcePromise?.SetHasRejectHandler();
    }
}

public class TaskPromise<T> : TaskPromise, IPromise<T>
{
    private readonly TaskCompletionSource<T> _tcs = new();
    private bool _resolved;
    private bool _rejected;

    public TaskPromise(PromiseRunHandler<T> run)
    {
        run(r =>
        {
            if (_resolved) return;
            _resolved = true;
            _tcs.SetResult(r);
        }, ex =>
        {
            if (_rejected) return;
            _rejected = true;
            ex ??= new Exception("Promise rejected without exception");
            
            if (HasRejectHandler)
            {
                _tcs.SetException(ex);
            }
            else
            {
                throw ex;
            }
        });
    }

    public Task<T> AsValueTask() => _tcs.Task;
    Task<T> IPromise<T>.AsTask() => AsValueTask();
    public override Task AsTask() => AsValueTask();

    public IPromise<Unit> Then(Action<T> onResolve) => Then(onResolve, null);
    public IPromise<TOut> Then<TOut>(Func<T, TOut> onResolve) => Then(onResolve, null);

    public IPromise<Unit> Then(Action<T> onResolve, Action<Exception> onReject)
    {
        if (onReject != null) SetHasRejectHandler();
        return new TaskPromise<Unit>((resolve, reject) =>
        {
            AsValueTask().ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    var ex = t.Exception!.InnerException;
                    if (onReject == null)
                    {
                        reject(ex);
                    }
                    else
                    {
                        onReject(ex);
                        resolve(Unit.Instance);
                    }
                }
                else
                {
                    onResolve?.Invoke(t.Result);
                    resolve(Unit.Instance);
                }
            });
        })
        {
            SourcePromise = this
        };
    }

    public IPromise<TOut> Then<TOut>(Func<T, TOut> onResolve, Func<Exception, TOut> onReject)
    {
        if (onResolve == null && onReject == null) 
            throw new ArgumentException("onResolve and onReject cannot both be null");
        
        if (onReject != null) SetHasRejectHandler();
        return new TaskPromise<TOut>((resolve, reject) =>
        {
            AsValueTask().ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    var ex = t.Exception!.InnerException;
                    if (onReject != null)
                    {
                        // Run onReject and resolve with the result
                        resolve(onReject(ex));
                    }
                    else
                    {
                        // Reject with the exception
                        reject(ex);
                    }
                }
                else
                {
                    // Resolved
                    if (onResolve != null)
                    {
                        resolve(onResolve(t.Result));
                    }
                    else
                    {
                        // Can occur when calling Catch(Func<Exception, TOut>).
                        // In this case we resolve with the default value of TOut.
                        resolve(default);
                    }
                }
            });
        })
        {
            SourcePromise = this
        };
    }

    public IPromise<Unit> Catch(Action<Exception> onReject) => Then(null, onReject);
    public IPromise<TOut> Catch<TOut>(Func<Exception, TOut> onReject) => Then(null, onReject);
}