using System.Diagnostics;
using System.Globalization;

namespace Mochi.ObjC;

[DebuggerDisplay("{ToString()}")]
public struct CGFloat
{
    public readonly double Value;

    public CGFloat(double value)
    {
        Value = value;
    }

    public static implicit operator CGFloat(double val) => new(val);
    public static implicit operator double(CGFloat val) => val.Value;

    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}