using System.Diagnostics.CodeAnalysis;

namespace Mochi.ObjC;

public interface IObjCInterface<out T> : INativeHandle<T>
{
    public static abstract ObjCClass RuntimeClass { get; }
    
    public TOut Cast<TOut>() where TOut : IObjCInterface<TOut>
    {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        if (!TryCast(out TOut result))
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            throw new Exception("Cannot cast to " + typeof(T).Name);

        return result;
    }
    
    public bool TryCast<TOut>([MaybeNullWhen(false)] out TOut result) where TOut : IObjCInterface<TOut>
    {
        var obj = new NSObject(Handle);
        if (obj.IsKindOfClass(TOut.RuntimeClass))
        {
            result = TOut.CreateWithHandle(Handle);
            return true;
        }

        result = default;
        return false;
    }
}