using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using KaLib.Structs;

namespace KaLib.Utils;

public static class MacOsUtil
{
    private const string ApplicationServicesLib =
        "/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices";
    
    [DllImport(ApplicationServicesLib)]
    private static extern void GetCurrentProcess(out ProcessSerialNumber serialNumber);
    
    [DllImport(ApplicationServicesLib, CharSet = CharSet.Auto)]
    private static extern void CPSSetProcessName(ref ProcessSerialNumber serialNumber, string name);

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