using System.Runtime.CompilerServices;
using Mochi.ObjC;

namespace Mochi.Metal;

public struct MTLResource : INativeHandle<MTLResource>, IMTLResource
{
    private readonly IntPtr Handle;
    IntPtr INativeHandle<MTLResource>.Handle => Handle;
    public MTLResource(IntPtr handle) => Handle = handle;

    public NSString Label
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetNSStringDelegate>()(Handle, 
                CommonSelectors.Label);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetNSStringDelegate>()(Handle, 
                CommonSelectors.SetLabel, value);
        }
    }
    
    public MTLDevice Device
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetDeviceDelegate>()(Handle, _selGetDevice);
        }
    }
    
    static MTLResource INativeHandle<MTLResource>.CreateWithHandle(IntPtr handle) => new(handle);

    private static readonly Selector _selGetDevice = "device";
}

public static class MTLResourceExtension 
{
    public static unsafe ref MTLResource AsMTLResource<T>(this ref T resource) where T : struct, INativeHandle<T>, IMTLResourceLike
    {
        var ptr = Unsafe.AsPointer(ref resource);
        return ref Unsafe.AsRef<MTLResource>(ptr);
    }
}