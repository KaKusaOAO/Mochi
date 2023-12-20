using System.Runtime.CompilerServices;
using System.Text;

namespace Mochi.ObjC;

public unsafe struct ObjCClass : INativeHandle<ObjCClass>
{
    public readonly IntPtr Handle;
    IntPtr INativeHandle<ObjCClass>.Handle => Handle;

    private delegate IntPtr AllocDelegate(IntPtr clz, Selector selector);
    private delegate IntPtr InitDelegate(IntPtr ptr, Selector selector);

    public ObjCClass(IntPtr handle)
    {
        Handle = handle;
    }

    public ObjCClass(string name)
    {
        var bytes = Encoding.UTF8.GetBytes(name);
        fixed (byte* ptr = bytes)
        {
            Handle = Native.objc_getClass(ptr);
        }
    }

    public T Alloc<T>()
    {
        this.EnsureInstanceNotNull();
        var result = ObjCRuntime.GetSendMessageFunction<AllocDelegate>()(Handle, Selectors.Alloc);
        return Unsafe.AsRef<T>(&result);
    }

    public T AllocInit<T>()
    {
        this.EnsureInstanceNotNull();
        var result = ObjCRuntime.GetSendMessageFunction<AllocDelegate>()(Handle, Selectors.Alloc);
        result = ObjCRuntime.GetSendMessageFunction<InitDelegate>()(result, Selectors.Init);
        return Unsafe.AsRef<T>(&result);
    }
    
    public static implicit operator ObjCClass(string s) => new(s);

    static ObjCClass INativeHandle<ObjCClass>.CreateWithHandle(IntPtr handle) => new(handle);
}