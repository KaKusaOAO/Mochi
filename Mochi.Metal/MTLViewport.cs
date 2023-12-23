using System.Runtime.InteropServices;

namespace Mochi.Metal;

[StructLayout(LayoutKind.Sequential)]
public struct MTLViewport
{
    public double OriginX;
    public double OriginY;
    public double Width;
    public double Height;
    public double ZNear;
    public double ZFar;
}