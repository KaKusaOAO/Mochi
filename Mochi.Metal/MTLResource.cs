using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mochi.ObjC;

namespace Mochi.Metal;

[StructLayout(LayoutKind.Sequential)]
public struct MTLResource : INativeHandle<MTLResource>, IMTLResource
{
    public readonly IntPtr Handle;
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
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetDeviceDelegate>()(Handle, "device");
        }
    }
    
    static MTLResource INativeHandle<MTLResource>.CreateWithHandle(IntPtr handle) => new(handle);
}

public static class MTLResourceExtension 
{
    public static MTLResource AsMTLResource<T>(this ref T resource)
        where T : struct, INativeHandle<T>, IMTLResourceLike =>
        resource.UnsafeCast<T, MTLResource>();
}