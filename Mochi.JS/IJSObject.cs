namespace Mochi.JS;

public interface IJSObject
{
    public object Handle { get; }
}

public static class JSObjectExtensions
{
    public static T As<T>(this IJSObject obj) where T : IJSObject => HandlePool<T>.Shared.Get(obj.Handle);
}