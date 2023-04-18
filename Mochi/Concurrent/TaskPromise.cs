using System;
using System.Threading.Tasks;
using Mochi.Core;

namespace Mochi.Concurrent;

public abstract class TaskPromise : IPromise
{
    protected bool HasRejectHandler { get; set; }
    protected TaskPromise SourcePromise { get; set; }
    public abstract Task AsTask();
    public abstract IPromise Then(Action onResolve);
    public abstract IPromise<TOut> Then<TOut>(Func<IPromise<TOut>> onResolve);
    public abstract IPromise<TOut> Then<TOut>(Func<TOut> onResolve);
    public abstract IPromise Then(Action onResolve, Action<Exception> onReject);
    public abstract IPromise<TOut> Then<TOut>(Func<IPromise<TOut>> onResolve, Func<Exception, IPromise<TOut>> onReject);
    public abstract IPromise<TOut> Then<TOut>(Func<TOut> onResolve, Func<Exception, TOut> onReject);
    public abstract IPromise Catch(Action<Exception> onReject);
    public abstract IPromise<TOut> Catch<TOut>(Func<Exception, TOut> onReject);

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

    public override IPromise Then(Action onResolve) => Then(_ => onResolve());

    public override IPromise<TOut> Then<TOut>(Func<IPromise<TOut>> onResolve) => Then(_ => onResolve());

    public override IPromise<TOut> Then<TOut>(Func<TOut> onResolve) => Then(_ => onResolve());

    public override IPromise Then(Action onResolve, Action<Exception> onReject) => 
        Then(_ => onResolve(), onReject);

    public override IPromise<TOut> Then<TOut>(Func<IPromise<TOut>> onResolve, Func<Exception, IPromise<TOut>> onReject) => 
        Then(_ => onResolve(), onReject);

    public override IPromise<TOut> Then<TOut>(Func<TOut> onResolve, Func<Exception, TOut> onReject) => 
        Then(_ => onResolve(), onReject);

    public override Task AsTask() => AsValueTask();

    public IPromise Then(Action<T> onResolve) => Then(onResolve, null);
    public IPromise<TOut> Then<TOut>(Func<T, IPromise<TOut>> onResolve) => Then(onResolve, null);
    public IPromise<TOut> Then<TOut>(Func<T, TOut> onResolve) => Then(onResolve, null);

    public IPromise Then(Action<T> onResolve, Action<Exception> onReject)
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

    public IPromise<TOut> Then<TOut>(Func<T, IPromise<TOut>> onResolve, Func<Exception, IPromise<TOut>> onReject)
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
                        onReject(ex).AsTask().ContinueWith(t2 =>
                        {
                            if (t2.IsFaulted)
                            {
                                reject(t2.Exception!.InnerException);
                            }
                            else
                            {
                                resolve(t2.Result);
                            }
                        });
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
                        // Run onResolve and resolve with the result
                        onResolve(t.Result).AsTask().ContinueWith(t2 =>
                        {
                            if (t2.IsFaulted)
                            {
                                reject(t2.Exception!.InnerException);
                            }
                            else
                            {
                                resolve(t2.Result);
                            }
                        });
                    }
                    else
                    {
                        // Can occur when calling Catch(Func<Exception, IPromise<TOut>>)
                        resolve(default!);
                    }
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

    public override IPromise Catch(Action<Exception> onReject) => Then(null, onReject);
    public override IPromise<TOut> Catch<TOut>(Func<Exception, TOut> onReject) => Then(null, onReject);
}