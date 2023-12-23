using System.Runtime.Versioning;

namespace Mochi.Metal;

public enum MTLVisibilityResultMode : uint
{
    Disabled,
    Boolean,
    
    [SupportedOSPlatform("macos10.11")]
    [SupportedOSPlatform("ios9.0")]
    Counting
}