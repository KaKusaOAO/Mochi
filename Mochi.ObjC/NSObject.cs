namespace Mochi.ObjC;

public struct NSObject : IObjCInterface<NSObject>
{
    public readonly IntPtr Handle;
    IntPtr INativeHandle<NSObject>.Handle => Handle;

    public NSObject(IntPtr ptr) => Handle = ptr;

    public bool IsKindOfClass(ObjCClass clz)
    {
        this.EnsureInstanceNotNull();
        return ObjCRuntime.GetSendMessageFunction<IsKindOfClassDelegate>()(Handle, _selIsKindOfClass, clz.Handle);
    }

    static NSObject INativeHandle<NSObject>.CreateWithHandle(IntPtr handle) => new(handle);

    private delegate NativeTypes.Bool8 IsKindOfClassDelegate(IntPtr ptr, Selector sel, IntPtr clz);

    public static ObjCClass RuntimeClass { get; } = nameof(NSObject);
    private static readonly Selector _selIsKindOfClass = "isKindOfClass:";
}