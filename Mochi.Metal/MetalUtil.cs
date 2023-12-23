using System.Runtime.Versioning;

namespace Mochi.Metal;

public static class MetalUtil
{
    /// <summary>
    /// Check if Metal is likely to be supported on this platform.
    /// </summary>
    public static bool IsOSSupported => 
        OperatingSystem.IsMacOSVersionAtLeast(10, 11) || OperatingSystem.IsIOSVersionAtLeast(8);
}