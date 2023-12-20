using System.Runtime.Versioning;

namespace Mochi.Metal;

[SupportedOSPlatform("macos10.11")]
[SupportedOSPlatform("ios9.0")]
public enum MTLLanguageVersion : uint
{
    [ObsoletedOSPlatform("ios16.0", "Use a newer language standard.")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("maccatalyst")]
    Version1_0 = 1 << 16,
    
    [SupportedOSPlatform("macos10.11")]
    [SupportedOSPlatform("ios9.0")]
    Version1_1,
    
    [SupportedOSPlatform("macos10.12")]
    [SupportedOSPlatform("ios10.0")]
    Version1_2,
    
    [SupportedOSPlatform("macos10.13")]
    [SupportedOSPlatform("ios11.0")]
    Version2_0 = 2 << 16,
    
    [SupportedOSPlatform("macos10.14")]
    [SupportedOSPlatform("ios12.0")]
    Version2_1,
    
    [SupportedOSPlatform("macos10.15")]
    [SupportedOSPlatform("ios13.0")]
    Version2_2,
    
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("ios14.0")]
    Version2_3,
    
    [SupportedOSPlatform("macos12.0")]
    [SupportedOSPlatform("ios15.0")]
    Version2_4,
    
    [SupportedOSPlatform("macos13.0")]
    [SupportedOSPlatform("ios16.0")]
    Version3_0 = 3 << 16,
    
    [SupportedOSPlatform("macos14.0")]
    [SupportedOSPlatform("ios17.0")]
    Version3_1
}