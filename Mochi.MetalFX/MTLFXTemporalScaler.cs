using System.Runtime.Versioning;
using Mochi.Metal;
using Mochi.ObjC;

namespace Mochi.MetalFX;

public struct MTLFXTemporalScaler : INativeHandle<MTLFXTemporalScaler>
{
    private readonly IntPtr Handle;
    IntPtr INativeHandle<MTLFXTemporalScaler>.Handle => Handle;

    public MTLFXTemporalScaler(IntPtr handle) => Handle = handle;

    public void EncodeToCommandBuffer(MTLCommandBuffer cb)
    {
        this.EnsureInstanceNotNull();
        ObjCRuntime.GetSendMessageFunction<EncodeToCommandBufferDelegate>()(Handle, _selEncodeToCommandBuffer, cb);
    }

    private delegate void EncodeToCommandBufferDelegate(IntPtr handle, Selector sel, MTLCommandBuffer cb);

    static MTLFXTemporalScaler INativeHandle<MTLFXTemporalScaler>.CreateWithHandle(IntPtr handle) => new(handle);

    private static readonly Selector _selEncodeToCommandBuffer = "encodeToCommandBuffer:";
}