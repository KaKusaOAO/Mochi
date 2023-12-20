namespace Mochi.ObjC;

public struct NSRange
{
    public UIntPtr Location;
    public UIntPtr Length;

    public NSRange(UIntPtr location, UIntPtr length)
    {
        Location = location;
        Length = length;
    }
    
    public NSRange(uint location, uint length)
    {
        Location = location;
        Length = length;
    }
}