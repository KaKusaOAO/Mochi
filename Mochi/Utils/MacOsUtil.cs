#if NET7_0_OR_GREATER
#define USE_LIBRARY_IMPORT
#endif

using System;
using System.Runtime.InteropServices;
using Mochi.Structs;

namespace Mochi.Utils;

#if USE_LIBRARY_IMPORT
public static partial class MacOsUtil
#else
public static class MacOsUtil
#endif
{
    private const string ApplicationServicesLib =
        "/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices";
    
#if USE_LIBRARY_IMPORT
    [LibraryImport(ApplicationServicesLib)]
    private static partial void GetCurrentProcess(out ProcessSerialNumber serialNumber);
    
    [LibraryImport(ApplicationServicesLib, StringMarshalling = StringMarshalling.Utf8)]
    private static partial void CPSSetProcessName(ref ProcessSerialNumber serialNumber, string name);
#else
    [DllImport(ApplicationServicesLib)]
    private static extern void GetCurrentProcess(out ProcessSerialNumber serialNumber);
    
    [DllImport(ApplicationServicesLib, CharSet = CharSet.Auto)]
    private static extern void CPSSetProcessName(ref ProcessSerialNumber serialNumber, string name);
#endif

    public static void SetAppMenuName(string name)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
#if DEBUG
            throw new PlatformNotSupportedException();
#else
            return;
#endif
        
        GetCurrentProcess(out var serialNumber);
        CPSSetProcessName(ref serialNumber, name);
    }
}