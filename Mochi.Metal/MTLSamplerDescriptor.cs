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
                _propLodAverage.Getter);
        }

        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetBoolDelegate>()(Handle,
                _propLodAverage.Setter, value);
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
                _propSupportArgumentBuffers.Getter);
        }

        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetBoolDelegate>()(Handle,
                _propSupportArgumentBuffers.Setter, value);
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

    private static readonly Property _propSupportArgumentBuffers = Property.Create("supportArgumentBuffers");
    private static readonly Property _propLodAverage = Property.Create("lodAverage");
}