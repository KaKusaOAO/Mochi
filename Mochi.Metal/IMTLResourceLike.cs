using Mochi.ObjC;

namespace Mochi.Metal;

public interface IMTLResourceLike
{
    public NSString Label { get; set; }
    public MTLDevice Device { get; }
}