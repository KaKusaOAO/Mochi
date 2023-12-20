using System.Runtime.Versioning;

namespace Mochi.Metal;

[SupportedOSPlatform("macos13.0")]
[SupportedOSPlatform("ios16.0")]
public enum MTLLibraryOptimizationLevel
{
    Default,
    Size
}