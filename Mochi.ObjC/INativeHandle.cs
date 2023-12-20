namespace Mochi.ObjC;

public interface INativeHandle<out T>
{
    IntPtr Handle { get; }

    public bool IsNull => Handle == 0;

    public void EnsureInstanceNotNull()
    {
        if (IsNull) throw new NullReferenceException();
    }

    protected static abstract T CreateWithHandle(IntPtr handle);
}

public static class NativeHandleExtension
{
    public static bool IsNull<T>(this INativeHandle<T> handle) => handle.IsNull;
    public static void EnsureInstanceNotNull<T>(this INativeHandle<T> handle) => handle.EnsureInstanceNotNull();
}