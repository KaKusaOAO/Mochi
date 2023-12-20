using System.Runtime.Versioning;

namespace Mochi.Metal;

[SupportedOSPlatform("macos10.11")]
[SupportedOSPlatform("ios9.0")]
public enum MTLDepthClipMode : uint
{
    Clip,
    Clamp
}