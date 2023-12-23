using Mochi.ObjC;

namespace Mochi.Metal;

public struct CALayer : IObjCInterface<CALayer>
{
    public readonly IntPtr Handle;
    IntPtr INativeHandle<CALayer>.Handle => Handle;

    public CALayer(IntPtr handle) => Handle = handle;
    static CALayer INativeHandle<CALayer>.CreateWithHandle(IntPtr handle) => new(handle);

    public void AddSublayer(IntPtr handle)
    {
        this.EnsureInstanceNotNull();
        ObjCRuntime.GetSendMessageFunction<AddSublayerDelegate>()(Handle, "addSublayer:", handle);
    }

    public static ObjCClass RuntimeClass { get; } = nameof(CALayer);

    private delegate void AddSublayerDelegate(IntPtr handle, Selector sel, IntPtr layer);
}