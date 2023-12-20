using System.Runtime.Versioning;
using Mochi.ObjC;

namespace Mochi.Metal;

public struct MTLRenderPipelineDescriptor : IObjCInterface<MTLRenderPipelineDescriptor>
{
    public readonly IntPtr Handle;
    IntPtr INativeHandle<MTLRenderPipelineDescriptor>.Handle => Handle;
    public MTLRenderPipelineDescriptor(IntPtr handle) => Handle = handle;

    public NSString Label
    {
        get => ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetNSStringDelegate>()(Handle, 
            CommonSelectors.Label);
        set => ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetNSStringDelegate>()(Handle,
            CommonSelectors.SetLabel, value);
    }

    public static ObjCClass RuntimeClass { get; } = nameof(MTLRenderPipelineDescriptor);
    static MTLRenderPipelineDescriptor INativeHandle<MTLRenderPipelineDescriptor>.CreateWithHandle(IntPtr handle) => new(handle);
}