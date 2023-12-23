using Mochi.ObjC;

namespace Mochi.Metal;

public interface IMTLResourceLike
{
    /// <summary>
    /// A string to help identify this object.
    /// </summary>
    public NSString Label { get; set; }
    
    /// <summary>
    /// The device this resource was created against.
    /// </summary>
    public MTLDevice Device { get; }
}