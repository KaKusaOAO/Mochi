using System.Runtime.Versioning;

namespace Mochi.Metal;

[SupportedOSPlatform("macos10.12")]
[SupportedOSPlatform("ios10.0")]
public enum MTLTessellationControlPointIndexType : uint
{
    None,
    UInt16,
    UInt32
}