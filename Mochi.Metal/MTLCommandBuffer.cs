using Mochi.ObjC;

namespace Mochi.Metal;

public struct MTLCommandBuffer : INativeHandle<MTLCommandBuffer>, IMTLResourceLike
{
    private readonly IntPtr Handle;
    IntPtr INativeHandle<MTLCommandBuffer>.Handle => Handle;

    public MTLCommandBuffer(IntPtr handle) => Handle = handle;

    public MTLDevice Device => this.AsMTLResource().Device;

    public NSString Label
    {
        get => this.AsMTLResource().Label;
        set
        {
            var r = this.AsMTLResource();
            r.Label = value;
        }
    }

    static MTLCommandBuffer INativeHandle<MTLCommandBuffer>.CreateWithHandle(IntPtr handle) => new(handle);
}