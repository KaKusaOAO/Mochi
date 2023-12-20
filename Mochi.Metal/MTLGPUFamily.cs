using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

namespace Mochi.Metal;

[SupportedOSPlatform("macos10.15")]
[SupportedOSPlatform("ios13.0")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum MTLGPUFamily : uint
{
    Apple1 = 1001,
    Apple2,
    Apple3,
    Apple4,
    Apple5,
    Apple6,
    Apple7,
    Apple8,
    Apple9,
    
    [ObsoletedOSPlatform("macos13.0", "Use MTLGPUFamily.Mac2")]
    [ObsoletedOSPlatform("ios16.0", "Use MTLGPUFamily.Mac2")]
    Mac1 = 2001,
    Mac2,
    
    Common1 = 3001,
    Common2,
    Common3,
    
    [ObsoletedOSPlatform("macos13.0", "Use MTLGPUFamily.Mac2")]
    [ObsoletedOSPlatform("ios16.0", "Use MTLGPUFamily.Mac2")]
    MacCatalyst1 = 4001,
    [ObsoletedOSPlatform("macos13.0", "Use MTLGPUFamily.Mac2")]
    [ObsoletedOSPlatform("ios16.0", "Use MTLGPUFamily.Mac2")]
    MacCatalyst2,
    
    [SupportedOSPlatform("macos13.0")]
    [SupportedOSPlatform("ios16.0")]
    Metal3 = 5001
}