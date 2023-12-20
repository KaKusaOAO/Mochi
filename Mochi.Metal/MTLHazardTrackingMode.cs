using System.Runtime.Versioning;

namespace Mochi.Metal;

[SupportedOSPlatform("macos10.15")]
[SupportedOSPlatform("ios13.0")]
public enum MTLHazardTrackingMode : uint
{
    Default,
    Untracked,
    Tracked
}