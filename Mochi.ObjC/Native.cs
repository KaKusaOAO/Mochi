using System.Runtime.InteropServices;
using System.Text;

namespace Mochi.ObjC;

internal static unsafe class Native
{
    private const string DllName = "/usr/lib/libobjc.A.dylib";
    
    public static bool IsSupported { get; private set; }
    
    public static IntPtr MsgSendPointer { get; private set; }

    [DllImport(DllName)]
    public static extern IntPtr sel_registerName(byte* namePtr);
    
    [DllImport(DllName)]
    public static extern byte* sel_getName(IntPtr selector);
    
    [DllImport(DllName)]
    public static extern IntPtr objc_getClass(byte* namePtr);
    
    static Native()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return;

        IsSupported = true;
        
        var bytes = Encoding.UTF8.GetBytes(DllName);
        fixed (byte* dllNamePtr = bytes)
        {
            var handle = LibDl.dlopen(dllNamePtr, LibDl.OpenFlags.Lazy);
            if (handle == 0)
                throw new Exception("Failed to load " + DllName + " with dlopen()");

            try
            {
                var symBytes = "objc_msgSend"u8;
                fixed (byte* symNamePtr = symBytes)
                {
                    var sym = LibDl.dlsym(handle, symNamePtr);
                    if (sym == 0)
                        throw new Exception("Failed to load objc_msgSend with dlsym()");

                    MsgSendPointer = sym;
                }
            }
            finally
            {
                LibDl.dlclose(handle);
            }
        }
    }
}