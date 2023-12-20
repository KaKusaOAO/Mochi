using System;
using System.Threading.Tasks;
using Mochi.Core;

namespace Mochi.Concurrent;

public interface IPromise
{
    public Task AsTask();

    public IPromise Then(Action onResolve);
    public IPromise<TOut> Then<TOut>(Func<IPromise<TOut>> onResolve);
    public IPromise<TOut> Then<TOut>(Func<TOut> onResolve);
    
    public IPromise Then(Action onResolve, Action<Exception> onReject);
    public IPromise<TOut> Then<TOut>(Func<IPromise<TOut>> onResolve, Func<Exception, IPromise<TOut>> onReject);
    public IPromise<TOut> Then<TOut>(Func<TOut> onResolve, Func<Exception, TOut> onReject);
    
    public IPromise Catch(Action<Exception> onReject);
    public IPromise<TOut> Catch<TOut>(Func<Exception, TOut> onReject);

    public static IPromise Resolved() => Resolved(Unit.Instance);
    public static IPromise<T> Resolved<T>(T val) => 
        MochiLibrary.Platform.CreatePromise<T>((resolve, _) => resolve(val));
    public static IPromise Rejected(Exception ex) => Rejected<Unit>(ex);
    public static IPromise<T> Rejected<T>(Exception ex) => 
        MochiLibrary.Platform.CreatePromise<T>((_, reject) => reject(ex));
}

public interface IPromise<T> : IPromise
{
    public new Task<T> AsTask();
    Task IPromise.AsTask() => AsTask();
    
    public IPromise Then(Action<T> onResolve);
    IPromise IPromise.Then(Action onResolve) => Then(_ => onResolve());

    public IPromise<TOut> Then<TOut>(Func<T, IPromise<TOut>> onResolve);
    IPromise<TOut> IPromise.Then<TOut>(Func<IPromise<TOut>> onResolve) => Then(_ => onResolve());

    public IPromise<TOut> Then<TOut>(Func<T, TOut> onResolve);
    IPromise<TOut> IPromise.Then<TOut>(Func<TOut> onResolve) => Then(_ => onResolve());

    public IPromise Then(Action<T> onResolve, Action<Exception> onReject);
    IPromise IPromise.Then(Action onResolve, Action<Exception> onReject) => Then(_ => onResolve(), onReject);

    public IPromise<TOut> Then<TOut>(Func<T, IPromise<TOut>> onResolve, Func<Exception, IPromise<TOut>> onReject);
    IPromise<TOut> IPromise.Then<TOut>(Func<IPromise<TOut>> onResolve, Func<Exception, IPromise<TOut>> onReject) =>
        Then(_ => onResolve(), onReject);

    public IPromise<TOut> Then<TOut>(Func<T, TOut> onResolve, Func<Exception, TOut> onReject);
    IPromise<TOut> IPromise.Then<TOut>(Func<TOut> onResolve, Func<Exception, TOut> onReject) =>
        Then(_ => onResolve(), onReject);
}

public delegate void PromiseResolveHandler<in T>(T value);
public delegate void PromiseRejectHandler(Exception ex = null);
public delegate void PromiseRunHandler<out T>(PromiseResolveHandler<T> resolve, PromiseRejectHandler reject);