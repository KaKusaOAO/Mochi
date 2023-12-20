using Mochi.ObjC;

namespace Mochi.Metal;

public struct MTLArchitecture : IObjCInterface<MTLArchitecture>
{
    private readonly IntPtr Handle;
    IntPtr INativeHandle<MTLArchitecture>.Handle => Handle;

    public MTLArchitecture(IntPtr handle) => Handle = handle;
    
    public NSString Name
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetNSStringDelegate>()(Handle, 
                CommonSelectors.Name);
        }
    }

    static MTLArchitecture INativeHandle<MTLArchitecture>.CreateWithHandle(IntPtr handle) => new(handle);
    public static ObjCClass RuntimeClass { get; } = nameof(MTLArchitecture);
}