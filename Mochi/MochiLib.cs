using Mochi.Core;

namespace Mochi;

public static class MochiLib
{
    public static IPlatform Platform { get; set; } = new DefaultPlatform();
}