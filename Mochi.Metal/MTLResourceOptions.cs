using System.Runtime.Versioning;

namespace Mochi.Metal;

[Flags]
public enum MTLResourceOptions : uint
{
    CPUDefaultCache  = MTLCPUCacheMode.DefaultCache,
    CPUWriteCombined = MTLCPUCacheMode.WriteCombined,
    
    [SupportedOSPlatform("macos10.11")]
    [SupportedOSPlatform("ios9.0")]
    StorageShared     = 0, // MTLStorageMode.Shared << 4,
    [SupportedOSPlatform("macos10.11")]
    [SupportedOSPlatform("maccatalyst13.0")]
    [UnsupportedOSPlatform("ios")]
    StorageManaged    = MTLStorageMode.Managed << 4,
    [SupportedOSPlatform("macos10.11")]
    [SupportedOSPlatform("ios9.0")]
    StoragePrivate    = MTLStorageMode.Private << 4,
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios10.0")]
    StorageMemoryless = MTLStorageMode.Memoryless << 4,
    
    [SupportedOSPlatform("macos10.15")]
    [SupportedOSPlatform("ios13.0")]
    TrackingDefault   = 0, // MTLHazardTrackingMode.Default << 8,
    [SupportedOSPlatform("macos10.15")]
    [SupportedOSPlatform("ios13.0")]
    TrackingUntracked = MTLHazardTrackingMode.Untracked << 8,
    [SupportedOSPlatform("macos10.15")]
    [SupportedOSPlatform("ios13.0")]
    TrackingTracked   = MTLHazardTrackingMode.Tracked << 8,
    
    [ObsoletedOSPlatform("macos13.0")]
    [ObsoletedOSPlatform("ios16.0")]
    CPUCacheModeDefault = CPUDefaultCache,
    [ObsoletedOSPlatform("macos13.0")]
    [ObsoletedOSPlatform("ios16.0")]
    CPUCacheModeWriteCombined = CPUWriteCombined
}