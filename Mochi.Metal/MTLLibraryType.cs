using System.Runtime.Versioning;

namespace Mochi.Metal;

[SupportedOSPlatform("macos11.0")]
[SupportedOSPlatform("ios14.0")]
public enum MTLLibraryType
{
    Executable,
    Dynamic
}