using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mochi.ObjC;

public static unsafe class LibDl
{
    private const string DllName = "/usr/lib/libdl.dylib";
    
    [Flags]
    public enum OpenFlags
    {
        Lazy = 0x1,
        Now = 0x2,
        Local = 0x4,
        Global = 0x8
    }

    [DllImport(DllName)]
    public static extern IntPtr dlopen(byte* name, OpenFlags flags);
    
    [DllImport(DllName)]
    public static extern IntPtr dlsym(IntPtr handle, byte* name);
    
    [DllImport(DllName)]
    public static extern int dlclose(IntPtr handle);
}