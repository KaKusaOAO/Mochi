using Mochi.ObjC;

namespace Mochi.Metal;

public struct MTLBinding : IObjCNativeHandle<MTLBinding>
{
    public readonly IntPtr Handle;
    IntPtr INativeHandle<MTLBinding>.Handle => Handle;

    public MTLBinding(IntPtr handle) => Handle = handle;

    public NSString Name
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetNSStringDelegate>()(Handle, 
                CommonSelectors.Name);
        }
    }

    static MTLBinding INativeHandle<MTLBinding>.CreateWithHandle(IntPtr handle) => new(handle);
}