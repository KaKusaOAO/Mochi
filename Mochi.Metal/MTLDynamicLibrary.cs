using Mochi.ObjC;

namespace Mochi.Metal;

public struct MTLDynamicLibrary : INativeHandle<MTLDynamicLibrary>, IMTLResourceLike
{
    public readonly IntPtr Handle;
    IntPtr INativeHandle<MTLDynamicLibrary>.Handle => Handle;

    public MTLDynamicLibrary(IntPtr handle) => Handle = handle;

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

    static MTLDynamicLibrary INativeHandle<MTLDynamicLibrary>.CreateWithHandle(IntPtr handle) => new(handle);
}