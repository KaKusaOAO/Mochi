namespace Mochi.Metal;

public struct MTLRegion
{
    public MTLOrigin Origin;
    public MTLSize Size;

    public MTLRegion(MTLOrigin origin, MTLSize size)
    {
        Origin = origin;
        Size = size;
    }
}