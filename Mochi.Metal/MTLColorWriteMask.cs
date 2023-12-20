using System.Runtime.Versioning;

namespace Mochi.Metal;

[Flags]
public enum MTLColorWriteMask : uint
{
    None,
    Red   = 1 << 3,
    Green = 1 << 2,
    Blue  = 1 << 1,
    Alpha = 1 << 0,
    All = 0xf
}