using System.Runtime.Versioning;

namespace Mochi.Metal;

[SupportedOSPlatform("macos10.11")]
[SupportedOSPlatform("ios9.0")]
public enum MTLStorageMode : uint
{
    Shared,
    
    [SupportedOSPlatform("macos10.11")]
    [SupportedOSPlatform("maccatalyst13.0")]
    [UnsupportedOSPlatform("ios")]
    Managed,
    
    Private,
    
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios10.0")]
    Memoryless
}