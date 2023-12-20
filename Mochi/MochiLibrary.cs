using Mochi.Core;

namespace Mochi;

public static class MochiLibrary
{
    public static IPlatform Platform { get; set; } = new DefaultPlatform();
}