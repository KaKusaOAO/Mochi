using System.Runtime.Versioning;
using Mochi.ObjC;

namespace Mochi.Metal;

public struct MTLVertexAttribute : IObjCInterface<MTLVertexAttribute>
{
    public readonly IntPtr Handle;
    IntPtr INativeHandle<MTLVertexAttribute>.Handle => Handle;

    public MTLVertexAttribute(IntPtr handle) => Handle = handle;

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
    
    [SupportedOSPlatform("macos10.12")]
    [SupportedOSPlatform("ios10.0")]
    public bool IsPatchData
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetBoolDelegate>()(Handle,
                MTLAttributeSelectors.IsPatchData);
        }
    }
    
    [SupportedOSPlatform("macos10.12")]
    [SupportedOSPlatform("ios10.0")]
    public bool IsPatchControlPointData
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetBoolDelegate>()(Handle,
                MTLAttributeSelectors.IsPatchControlPointData);
        }
    }

    static MTLVertexAttribute INativeHandle<MTLVertexAttribute>.CreateWithHandle(IntPtr handle) => new(handle);

    public static ObjCClass RuntimeClass { get; } = nameof(MTLVertexAttribute);
}