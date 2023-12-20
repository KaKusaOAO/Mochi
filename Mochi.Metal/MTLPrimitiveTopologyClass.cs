using System.Runtime.Versioning;

namespace Mochi.Metal;

[SupportedOSPlatform("macos10.11")]
[SupportedOSPlatform("ios12.0")]
public enum MTLPrimitiveTopologyClass : uint
{
    Unspecified,
    Point,
    Line,
    Triangle
}