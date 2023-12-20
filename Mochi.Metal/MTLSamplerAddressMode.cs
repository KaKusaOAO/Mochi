using System.Runtime.Versioning;

namespace Mochi.Metal;

public enum MTLSamplerAddressMode : uint
{
    ClampToEdge,
    
    [SupportedOSPlatform("macos10.11")]
    [SupportedOSPlatform("ios14.0")]
    MirrorClampToEdge,
    Repeat,
    MirrorRepeat,
    ClampToZero,
    
    [SupportedOSPlatform("macos10.12")]
    [SupportedOSPlatform("ios14.0")]
    ClampToBorderColor
}