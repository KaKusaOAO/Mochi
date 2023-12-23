using Mochi.ObjC;

namespace Mochi.Metal;

public unsafe struct MTLRenderCommandEncoder : IObjCNativeHandle<MTLRenderCommandEncoder>, IMTLCommandEncoder
{
    public readonly IntPtr Handle;
    IntPtr INativeHandle<MTLRenderCommandEncoder>.Handle => Handle;
    
    public MTLRenderCommandEncoder(IntPtr handle) => Handle = handle;

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
    
    public void EndEncoding() => this.AsMTLCommandEncoder().EndEncoding();

    public void SetRenderPipelineState(MTLRenderPipelineState pipelineState)
    {
        this.EnsureInstanceNotNull();
        ObjCRuntime.GetSendMessageFunction<SetRenderPipelineStateDelegate>()(Handle, "setRenderPipelineState:",
            pipelineState);
    }

    public void SetViewport(MTLViewport viewport)
    {
        this.EnsureInstanceNotNull();
        ObjCRuntime.GetSendMessageFunction<SetViewportDelegate>()(Handle, "setViewport:", viewport);
    }
    
    public void SetViewports(MTLViewport* viewports, uint count)
    {
        this.EnsureInstanceNotNull();
        ObjCRuntime.GetSendMessageFunction<SetViewportsDelegate>()(Handle, "setViewports:count:", viewports, count);
    }

    public void SetViewports(MTLViewport[] viewports)
    {
        fixed (MTLViewport* ptr = viewports)
        {
            SetViewports(ptr, (uint) viewports.Length);
        }
    }

    public void SetFrontFacingWinding(MTLWinding winding)
    {
        this.EnsureInstanceNotNull();
        ObjCRuntime.GetSendMessageFunction<SetWindingDelegate>()(Handle, "setFrontFacingWinding:", winding);
    }

    private delegate void SetRenderPipelineStateDelegate(IntPtr handle, Selector sel, MTLRenderPipelineState state);
    private delegate void SetViewportDelegate(IntPtr handle, Selector sel, MTLViewport viewport);
    private delegate void SetViewportsDelegate(IntPtr handle, Selector sel, MTLViewport* viewport, NSUInteger count);
    private delegate void SetWindingDelegate(IntPtr handle, Selector sel, MTLWinding winding);

    static MTLRenderCommandEncoder INativeHandle<MTLRenderCommandEncoder>.CreateWithHandle(IntPtr handle) => new(handle);
}