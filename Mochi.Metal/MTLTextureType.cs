using System.Runtime.Versioning;

namespace Mochi.Metal;

/// <summary>
/// <see cref="MTLTextureType"/> describes the dimensionality of each image, and if multiple images are arranged into an array or cube.
/// </summary>
public enum MTLTextureType : uint
{
    Type1D,
    Type1DArray,
    Type2D,
    Type2DArray,
    Type2DMultisample,
    TypeCube,
    
    [SupportedOSPlatform("macos10.11")]
    [SupportedOSPlatform("ios11.0")]
    TypeCubeArray,
    Type3D,
    
    [SupportedOSPlatform("macos10.14")]
    [SupportedOSPlatform("ios14.0")]
    Type2DMultisampleArray,
    
    [SupportedOSPlatform("macos10.14")]
    [SupportedOSPlatform("ios12.0")]
    TextureBuffer
}