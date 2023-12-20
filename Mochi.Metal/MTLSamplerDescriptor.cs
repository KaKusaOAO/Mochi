using System.Runtime.Versioning;
using Mochi.ObjC;

namespace Mochi.Metal;

public struct MTLSamplerDescriptor : IObjCInterface<MTLSamplerDescriptor>
{
    private IntPtr Handle;
    IntPtr INativeHandle<MTLSamplerDescriptor>.Handle => Handle;

    public MTLSamplerDescriptor(IntPtr handle) => Handle = handle;

    public static MTLSamplerDescriptor AllocInit() => RuntimeClass.AllocInit<MTLSamplerDescriptor>();

    [SupportedOSPlatform("ios9.0")]
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    public bool LodAverage
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetBoolDelegate>()(Handle,
                _selGetLodAverage);
        }

        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetBoolDelegate>()(Handle,
                _selSetLodAverage, value);
        }
    }
    
    [SupportedOSPlatform("macos10.13")]
    [SupportedOSPlatform("ios11.0")]
    public bool SupportsArgumentBuffers
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetBoolDelegate>()(Handle,
                _selGetSupportArgumentBuffers);
        }

        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetBoolDelegate>()(Handle,
                _selSetSupportArgumentBuffers, value);
        }
    }
    
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

    static MTLSamplerDescriptor INativeHandle<MTLSamplerDescriptor>.CreateWithHandle(IntPtr handle) => new(handle);

    public static ObjCClass RuntimeClass { get; } = nameof(MTLSamplerDescriptor);

    private static readonly Selector _selGetSupportArgumentBuffers = "supportArgumentBuffers";
    private static readonly Selector _selSetSupportArgumentBuffers = "setSupportArgumentBuffers:";
    private static readonly Selector _selGetLodAverage = "lodAverage";
    private static readonly Selector _selSetLodAverage = "setLodAverage:";
}