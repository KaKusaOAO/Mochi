using Mochi.Concurrent;

namespace Mochi.Core;

public interface IPlatform
{
    public IPromise<T> CreatePromise<T>(PromiseRunHandler<T> run);
}

public class DefaultPlatform : IPlatform
{
    public IPromise<T> CreatePromise<T>(PromiseRunHandler<T> run) => new TaskPromise<T>(run);
}