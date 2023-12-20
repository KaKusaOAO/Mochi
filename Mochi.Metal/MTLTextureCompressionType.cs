using System.Runtime.Versioning;

namespace Mochi.Metal;

[SupportedOSPlatform("macos12.5")]
[SupportedOSPlatform("ios15.0")]
public enum MTLTextureCompressionType
{
    Lossless,
    Lossy
}