using System.Runtime.Versioning;

namespace Mochi.Metal;

public enum MTLBlendFactor : uint
{
    Zero,
    One,
    SourceColor,
    OneMinusSourceColor,
    SourceAlpha,
    OneMinusSourceAlpha,
    DestinationColor,
    OneMinusDestinationColor,
    DestinationAlpha,
    OneMinusDestinationAlpha,
    SourceAlphaSaturated,
    BlendColor,
    OneMinusBlendColor,
    BlendAlpha,
    OneMinusBlendAlpha,  
    
    [SupportedOSPlatform("macos10.12")]
    [SupportedOSPlatform("ios10.11")]
    Source1Color,        
    [SupportedOSPlatform("macos10.12")]
    [SupportedOSPlatform("ios10.11")]
    OneMinusSource1Color,
    [SupportedOSPlatform("macos10.12")]
    [SupportedOSPlatform("ios10.11")]
    Source1Alpha,       
    [SupportedOSPlatform("macos10.12")]
    [SupportedOSPlatform("ios10.11")] 
    OneMinusSource1Alpha
}