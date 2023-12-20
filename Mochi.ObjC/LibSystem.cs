using System.Runtime.InteropServices;
using System.Text;

namespace Mochi.ObjC;

public static unsafe class LibSystem
{
    public static bool IsSupported { get; private set; }
    
    // ReSharper disable once InconsistentNaming
    public static IntPtr NSConcreteGlobalBlock { get; private set; }
    
    // ReSharper disable once InconsistentNaming
    public static IntPtr NSConcreteStackBlock { get; private set; }

    private static IntPtr LoadSymbol(IntPtr handle, string name)
    {
        var symBytes = Encoding.UTF8.GetBytes(name);
        fixed (byte* symNamePtr = symBytes)
        {
            return LibDl.dlsym(handle, symNamePtr);
        }
    }
    
    static LibSystem()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return;

        IsSupported = true;
        
        var bytes = "libSystem.dylib"u8;
        fixed (byte* dllNamePtr = bytes)
        {
            var handle = LibDl.dlopen(dllNamePtr, LibDl.OpenFlags.Lazy);
            if (handle == 0)
                throw new Exception("Failed to load libSystem.dylib with dlopen()");

            try
            {
                NSConcreteGlobalBlock = LoadSymbol(handle, "_NSConcreteGlobalBlock");
                NSConcreteStackBlock = LoadSymbol(handle, "_NSConcreteStackBlock");
            }
            finally
            {
                LibDl.dlclose(handle);
            }
        }
    }
}