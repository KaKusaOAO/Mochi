using System.Runtime.Versioning;

namespace Mochi.Metal;

[Flags]
[SupportedOSPlatform("macos10.11")]
[SupportedOSPlatform("ios9.0")]
public enum MTLTextureUsage : uint
{
    Unknown = 0,
    ShaderRead      = 1 << 0,
    ShaderWrite     = 1 << 1,
    RenderTarget    = 1 << 2,
    PixelFormatView = 1 << 4,
    
    [SupportedOSPlatform("macos14.0")]
    [SupportedOSPlatform("ios17.0")]
    ShaderAtomic    = 1 << 5
}