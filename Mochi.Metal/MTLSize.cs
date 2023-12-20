namespace Mochi.Metal;

public struct MTLSize
{
    public UIntPtr Width;
    public UIntPtr Height;
    public UIntPtr Depth;

    public MTLSize(uint width, uint height, uint depth)
    {
        Width = width;
        Height = height;
        Depth = depth;
    }
}