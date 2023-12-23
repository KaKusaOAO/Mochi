namespace Mochi.Metal;

public class IncompatibleTextureViewTypeException : IncompatibleTextureViewException
{
    public MTLTextureType SourceTextureType { get; }
    public MTLTextureType TextureViewType { get; }

    public IncompatibleTextureViewTypeException(MTLTextureType source, MTLTextureType view)
    {
        SourceTextureType = source;
        TextureViewType = view;
    }

    public override string Message =>
        $"Source texture type ({SourceTextureType}) not compatible with texture view type {TextureViewType}";
}