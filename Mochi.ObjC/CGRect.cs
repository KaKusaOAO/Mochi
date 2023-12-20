using System.Diagnostics;

namespace Mochi.ObjC;

[DebuggerDisplay("{ToString()}")]
public struct CGRect
{
    public CGPoint Origin;
    public CGSize Size;

    public CGRect(CGPoint origin, CGSize size)
    {
        Origin = origin;
        Size = size;
    }

    public override string ToString() => $"{Origin}, {Size}";
}