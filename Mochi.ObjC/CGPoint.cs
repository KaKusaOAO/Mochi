using System.Diagnostics;

namespace Mochi.ObjC;

[DebuggerDisplay("{ToString()}")]
public struct CGPoint
{
    public CGFloat X;
    public CGFloat Y;

    public CGPoint(double x, double y)
    {
        X = x;
        Y = y;
    }

    public override string ToString() => $"({X},{Y})";
}