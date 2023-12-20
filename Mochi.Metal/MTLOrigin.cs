namespace Mochi.Metal;

public struct MTLOrigin
{
    public UIntPtr X;
    public UIntPtr Y;
    public UIntPtr Z;

    public MTLOrigin(uint x, uint y, uint z)
    {
        X = x;
        Y = y;
        Z = z;
    }
}