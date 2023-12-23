using Mochi.ObjC;

namespace Mochi.Metal;

public struct MTLCommandQueue : INativeHandle<MTLCommandQueue>, IMTLResourceLike
{
    public readonly IntPtr Handle;
    IntPtr INativeHandle<MTLCommandQueue>.Handle => Handle;

    public MTLCommandQueue(IntPtr handle) => Handle = handle;
    
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

    public MTLCommandBuffer CreateCommandBuffer()
    {
        this.EnsureInstanceNotNull();
        return ObjCRuntime.GetSendMessageFunction<CommandBufferDelegate>()(Handle, _selCommandBuffer);
    }

    private delegate MTLCommandBuffer CommandBufferDelegate(IntPtr handle, Selector sel);

    static MTLCommandQueue INativeHandle<MTLCommandQueue>.CreateWithHandle(IntPtr handle) => new(handle);

    private static readonly Selector _selCommandBuffer = "commandBuffer";
}