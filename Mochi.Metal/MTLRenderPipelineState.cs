using Mochi.ObjC;

namespace Mochi.Metal;

public struct MTLRenderPipelineState : IObjCNativeHandle<MTLRenderPipelineState>, IMTLResourceLike
{
    public readonly IntPtr Handle;
    IntPtr INativeHandle<MTLRenderPipelineState>.Handle => Handle;
    
    public MTLRenderPipelineState(IntPtr handle) => Handle = handle;

    public NSString Label
    {
        get => this.AsMTLResource().Label;
        set
        {
            var r = this.AsMTLResource();
            r.Label = value;
        }
    }

    public MTLDevice Device => this.AsMTLResource().Device;

    static MTLRenderPipelineState INativeHandle<MTLRenderPipelineState>.CreateWithHandle(IntPtr handle) => new(handle);
}