using System;
using System.Threading.Tasks;
using Mochi.Core;

namespace Mochi.Concurrent;

public interface IPromise
{
    public Task AsTask();
    
    public static IPromise<T> Resolved<T>(T val) => 
        MochiLib.Platform.CreatePromise<T>((resolve, _) => resolve(val));
    public static IPromise<T> Rejected<T>(Exception ex) => 
        MochiLib.Platform.CreatePromise<T>((_, reject) => reject(ex));
    public static IPromise<Unit> Rejected(Exception ex) => Rejected<Unit>(ex);
}

public interface IPromise<T> : IPromise
{
    public new Task<T> AsTask();
    Task IPromise.AsTask() => AsTask();
    
    public IPromise<Unit> Then(Action<T> onResolve);
    public IPromise<TOut> Then<TOut>(Func<T, TOut> onResolve);
    
    public IPromise<Unit> Then(Action<T> onResolve, Action<Exception> onReject);
    public IPromise<TOut> Then<TOut>(Func<T, TOut> onResolve, Func<Exception, TOut> onReject);
    
    public IPromise<Unit> Catch(Action<Exception> onReject);
    public IPromise<TOut> Catch<TOut>(Func<Exception, TOut> onReject);
}

public static class PromiseExtension
{
    public static IPromise<Unit> Then(this IPromise<Unit> p, Action onResolve) => 
        p.Then(_ => onResolve());

    public static IPromise<TOut> Then<TOut>(this IPromise<Unit> p, Func<TOut> onResolve) =>
        p.Then(_ => onResolve());
}

public delegate void PromiseResolveHandler<in T>(T value);
public delegate void PromiseRejectHandler(Exception ex = null);
public delegate void PromiseRunHandler<out T>(PromiseResolveHandler<T> resolve, PromiseRejectHandler reject);