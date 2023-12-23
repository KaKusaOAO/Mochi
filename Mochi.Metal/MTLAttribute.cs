using System.Runtime.Versioning;
using Mochi.ObjC;

namespace Mochi.Metal;

[SupportedOSPlatform("macos10.12")]
[SupportedOSPlatform("ios10.0")]
public struct MTLAttribute : IObjCInterface<MTLAttribute>
{
    public readonly IntPtr Handle;
    IntPtr INativeHandle<MTLAttribute>.Handle => Handle;

    public MTLAttribute(IntPtr handle) => Handle = handle;

    public NSString Name
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetNSStringDelegate>()(Handle,
                MTLAttributeSelectors.Name);
        }
    }

    public uint AttributeIndex
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetNSUIntDelegate>()(Handle,
                MTLAttributeSelectors.AttributeIndex);
        }
    }
    
    public bool IsActive
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetBoolDelegate>()(Handle,
                MTLAttributeSelectors.IsActive);
        }
    }
    
    public bool IsPatchData
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetBoolDelegate>()(Handle,
                MTLAttributeSelectors.IsPatchData);
        }
    }
    
    public bool IsPatchControlPointData
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetBoolDelegate>()(Handle,
                MTLAttributeSelectors.IsPatchControlPointData);
        }
    }

    static MTLAttribute INativeHandle<MTLAttribute>.CreateWithHandle(IntPtr handle) => new(handle);

    public static ObjCClass RuntimeClass { get; } = nameof(MTLAttribute);
}