using System.Diagnostics;

namespace Mochi.ObjC;

[DebuggerDisplay("{ToString()}")]
public struct CGSize
{
    public double Width;
    public double Height;

    public CGSize(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public override string ToString() => $"{Width} x {Height}";
}