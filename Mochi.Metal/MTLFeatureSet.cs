using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

namespace Mochi.Metal;

[ObsoletedOSPlatform("macos10.13", "Use MTLGPUFamily instead.")]
[ObsoletedOSPlatform("ios16.0", "Use MTLGPUFamily instead.")]
[ObsoletedOSPlatform("tvos16.0", "Use MTLGPUFamily instead.")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum MTLFeatureSet
{
    [SupportedOSPlatform("ios8.0")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("maccatalyst")]
    [UnsupportedOSPlatform("tvos")]
    iOS_GPUFamily1_v1,
    [SupportedOSPlatform("ios8.0")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("maccatalyst")]
    [UnsupportedOSPlatform("tvos")]
    iOS_GPUFamily2_v1,
    
    [SupportedOSPlatform("ios9.0")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("maccatalyst")]
    [UnsupportedOSPlatform("tvos")]
    iOS_GPUFamily1_v2,
    [SupportedOSPlatform("ios9.0")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("maccatalyst")]
    [UnsupportedOSPlatform("tvos")]
    iOS_GPUFamily2_v2,
    [SupportedOSPlatform("ios9.0")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("maccatalyst")]
    [UnsupportedOSPlatform("tvos")]
    iOS_GPUFamily3_v1,
    
    [SupportedOSPlatform("ios10.0")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("maccatalyst")]
    [UnsupportedOSPlatform("tvos")]
    iOS_GPUFamily1_v3,
    [SupportedOSPlatform("ios10.0")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("maccatalyst")]
    [UnsupportedOSPlatform("tvos")]
    iOS_GPUFamily2_v3,
    [SupportedOSPlatform("ios10.0")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("maccatalyst")]
    [UnsupportedOSPlatform("tvos")]
    iOS_GPUFamily3_v2,
    
    [SupportedOSPlatform("ios11.0")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("maccatalyst")]
    [UnsupportedOSPlatform("tvos")]
    iOS_GPUFamily1_v4,
    [SupportedOSPlatform("ios11.0")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("maccatalyst")]
    [UnsupportedOSPlatform("tvos")]
    iOS_GPUFamily2_v4,
    [SupportedOSPlatform("ios11.0")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("maccatalyst")]
    [UnsupportedOSPlatform("tvos")]
    iOS_GPUFamily3_v3,
    [SupportedOSPlatform("ios11.0")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("maccatalyst")]
    [UnsupportedOSPlatform("tvos")]
    iOS_GPUFamily4_v1,
    
    [SupportedOSPlatform("ios12.0")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("maccatalyst")]
    [UnsupportedOSPlatform("tvos")]
    iOS_GPUFamily1_v5,
    [SupportedOSPlatform("ios12.0")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("maccatalyst")]
    [UnsupportedOSPlatform("tvos")]
    iOS_GPUFamily2_v5,
    [SupportedOSPlatform("ios12.0")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("maccatalyst")]
    [UnsupportedOSPlatform("tvos")]
    iOS_GPUFamily3_v4,
    [SupportedOSPlatform("ios12.0")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("maccatalyst")]
    [UnsupportedOSPlatform("tvos")]
    iOS_GPUFamily4_v2,
    [SupportedOSPlatform("ios12.0")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("maccatalyst")]
    [UnsupportedOSPlatform("tvos")]
    iOS_GPUFamily5_v1,
    
    [SupportedOSPlatform("macos10.11")]
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("maccatalyst")]
    macOS_GPUFamily1_v1 = 10000,
    [SupportedOSPlatform("macos10.12")]
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("maccatalyst")]
    macOS_GPUFamily1_v2,
    [SupportedOSPlatform("macos10.12")]
    [UnsupportedOSPlatform("ios")]
    macOS_ReadWriteTextureTier2,
    
    [SupportedOSPlatform("macos10.13")]
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("maccatalyst")]
    macOS_GPUFamily1_v3,
    [SupportedOSPlatform("macos10.14")]
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("maccatalyst")]
    macOS_GPUFamily1_v4,
    [SupportedOSPlatform("macos10.14")]
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("maccatalyst")]
    macOS_GPUFamily2_v1,
    
    [Obsolete("Use macOS_GPUFamily1_v1")]
    [SupportedOSPlatform("macos10.11")]
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("maccatalyst")]
    OSX_GPUFamily1_v1 = macOS_GPUFamily1_v1,
    [Obsolete("Use macOS_GPUFamily1_v2")]
    [SupportedOSPlatform("macos10.12")]
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("maccatalyst")]
    OSX_GPUFamily1_v2 = macOS_GPUFamily1_v2,
    [Obsolete("Use macOS_ReadWriteTextureTier2")]
    [SupportedOSPlatform("macos10.12")]
    [UnsupportedOSPlatform("ios")]
    OSX_ReadWriteTextureTier2 = macOS_ReadWriteTextureTier2,
    
    [SupportedOSPlatform("tvos9.0")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("ios")]
    tvOS_GPUFamily1_v1 = 30000,
    [SupportedOSPlatform("tvos10.0")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("ios")]
    tvOS_GPUFamily1_v2,
    [SupportedOSPlatform("tvos11.0")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("ios")]
    tvOS_GPUFamily1_v3,
    [SupportedOSPlatform("tvos11.0")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("ios")]
    tvOS_GPUFamily2_v1,
    [SupportedOSPlatform("tvos12.0")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("ios")]
    tvOS_GPUFamily1_v4,
    [SupportedOSPlatform("tvos12.0")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("ios")]
    tvOS_GPUFamily2_v2,
    
    [Obsolete]
    TVOS_GPUFamily1_v1 = tvOS_GPUFamily1_v1
}