using System.Globalization;

namespace Mochi.ObjC;

public struct NSUInteger
{
    public readonly UIntPtr Value;

    public NSUInteger(UIntPtr value)
    {
        Value = value;
    }

    public static implicit operator NSUInteger(UIntPtr val) => new(val);
    public static implicit operator NSUInteger(uint val) => new(val);
    public static implicit operator NSUInteger(ulong val) => new(UIntPtr.CreateTruncating(val));
    
    public static implicit operator UIntPtr(NSUInteger val) => val.Value;
    public static implicit operator uint(NSUInteger val) => checked((uint) val.Value);
    public static implicit operator ulong(NSUInteger val) => val.Value;

    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}