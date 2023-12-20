using System.Runtime.Versioning;

namespace Mochi.Metal;

[SupportedOSPlatform("macos13.3")]
[SupportedOSPlatform("ios16.4")]
public enum MTLCompileSymbolVisibility
{
    Default,
    Hidden
}