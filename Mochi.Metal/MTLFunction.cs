using System.Runtime.Versioning;
using Mochi.ObjC;

namespace Mochi.Metal;

[SupportedOSPlatform("macos10.11")]
[SupportedOSPlatform("ios8.0")]
public struct MTLFunction : IObjCInterface<MTLFunction>, IMTLResourceLike
{
    public readonly IntPtr Handle;
    IntPtr INativeHandle<MTLFunction>.Handle => Handle;

    public MTLFunction(IntPtr handle) => Handle = handle;
    
    /// <summary>
    /// A string to help identify this object.
    /// </summary>
    [SupportedOSPlatform("macos10.12")]
    [SupportedOSPlatform("ios10.0")]
    public NSString Label
    {
        get => this.AsMTLResource().Label;
        set
        {
            var r = this.AsMTLResource();
            r.Label = value;
        }
    }

    /// <summary>
    /// The device this resource was created against.
    /// This resource can only be used with this device.
    /// </summary>
    public MTLDevice Device => this.AsMTLResource().Device;
    
    /// <summary>
    /// The name of the function in the shading language.
    /// </summary>
    public NSString Name
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetNSStringDelegate>()(Handle, 
                CommonSelectors.Name);
        }
    }

    public static ObjCClass RuntimeClass { get; } = nameof(MTLFunction);
    static MTLFunction INativeHandle<MTLFunction>.CreateWithHandle(IntPtr handle) => new(handle);
}