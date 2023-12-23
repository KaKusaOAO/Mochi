namespace Mochi.Metal;

public class IncompatibleSliceRangeException : IncompatibleTextureViewException
{
    public int Provided { get; }
    public int? Expected { get; }
    public MTLTextureType Type { get; }
        
    public IncompatibleSliceRangeException(int provided, int expected, MTLTextureType type)
    {
        Provided = provided;
        Expected = expected;
        Type = type;
    }
        
    public IncompatibleSliceRangeException(int provided, MTLTextureType type)
    {
        Provided = provided;
        Type = type;
    }

    public override string Message => Expected.HasValue
        ? $"sliceRange.Length ({Provided}) must equal ({Expected.Value}) for texture type {Type}."
        : $"sliceRange.Length ({Provided}) not applicable for texture type {Type}.";
}