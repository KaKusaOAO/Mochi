using System.Runtime.Versioning;

namespace Mochi.Metal;

[SupportedOSPlatform("macos10.12")]
[SupportedOSPlatform("ios10.0")]
public enum MTLTessellationPartitionMode : uint
{
    Pow2,
    Integer,
    FractionalOdd,
    FractionalEven,
}