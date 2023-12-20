using System.Runtime.CompilerServices;

namespace Mochi.ObjC;

internal static class NSArraySelectors
{
    public static Selector Count { get; } = "count";
    public static Selector ObjectAtIndex { get; } = "objectAtIndex:";
}

public struct NSArray : IObjCInterface<NSArray>
{
    public readonly IntPtr Handle;
    IntPtr INativeHandle<NSArray>.Handle => Handle;

    public NSArray(IntPtr handle) => Handle = handle;

    public UIntPtr Count
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<GetCountDelegate>()(Handle, NSArraySelectors.Count);
        }
    }

    public IntPtr ObjectAtIndex(uint index)
    {
        this.EnsureInstanceNotNull();
        return ObjCRuntime.GetSendMessageFunction<GetObjectAtIndexDelegate>()(Handle, NSArraySelectors.ObjectAtIndex, index);
    }

    public IntPtr this[uint index] => ObjectAtIndex(index);

    public unsafe ref NSArray<T> AsTyped<T>() where T : INativeHandle<T>
    {
        var ptr = Unsafe.AsPointer(ref this);
        return ref Unsafe.AsRef<NSArray<T>>(ptr);
    }
    
    private delegate UIntPtr GetCountDelegate(IntPtr handle, Selector sel);
    private delegate IntPtr GetObjectAtIndexDelegate(IntPtr handle, Selector sel, uint index);
    
    public static ObjCClass RuntimeClass { get; } = nameof(NSArray);
    static NSArray INativeHandle<NSArray>.CreateWithHandle(IntPtr handle) => new(handle);
}

public struct NSArray<T> : IObjCInterface<NSArray<T>> where T : INativeHandle<T>
{
    public readonly IntPtr Handle;
    IntPtr INativeHandle<NSArray<T>>.Handle => Handle;

    public NSArray(IntPtr handle) => Handle = handle;

    public UIntPtr Count
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<GetCountDelegate>()(Handle, NSArraySelectors.Count);
        }
    }

    public T ObjectAtIndex(uint index)
    {
        this.EnsureInstanceNotNull();
        return ObjCRuntime.GetSendMessageFunction<GetObjectAtIndexDelegate>()(Handle, NSArraySelectors.ObjectAtIndex, index);
    }
    
    public T this[uint index] => ObjectAtIndex(index);
    
    public unsafe ref NSArray AsUntyped()
    {
        var ptr = Unsafe.AsPointer(ref this);
        return ref Unsafe.AsRef<NSArray>(ptr);
    }

    private delegate UIntPtr GetCountDelegate(IntPtr handle, Selector sel);
    private delegate T GetObjectAtIndexDelegate(IntPtr handle, Selector sel, uint index);

    public static ObjCClass RuntimeClass => NSArray.RuntimeClass;
    static NSArray<T> INativeHandle<NSArray<T>>.CreateWithHandle(IntPtr handle) => new(handle);
}