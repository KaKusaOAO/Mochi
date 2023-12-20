using System.Runtime.Versioning;

namespace Mochi.Metal;

[SupportedOSPlatform("macos10.12")]
[SupportedOSPlatform("ios14.0")]
public enum MTLSamplerBorderColor : uint
{
    TransparentBlack,
    OpaqueBlack,     
    OpaqueWhite    
}