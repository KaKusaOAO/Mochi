using Mochi.ObjC;

namespace Mochi.Metal;

public struct CAMetalLayer : IObjCInterface<CAMetalLayer>
{
    public readonly IntPtr Handle;
    IntPtr INativeHandle<CAMetalLayer>.Handle => Handle;

    public CAMetalLayer(IntPtr handle)
    {
        Handle = handle;
    }
    
    static CAMetalLayer INativeHandle<CAMetalLayer>.CreateWithHandle(IntPtr handle) => new(handle);

    public static CAMetalLayer AllocInit() => RuntimeClass.AllocInit<CAMetalLayer>();

    public static ObjCClass RuntimeClass { get; } = nameof(CAMetalLayer);

}