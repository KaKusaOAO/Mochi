using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using Mochi.ObjC;

namespace Mochi.Metal;

public interface IMTLType {}

public static class MTLTypeExtension
{
    [SupportedOSPlatform("macos10.13")]
    [SupportedOSPlatform("ios11.0")]
    public static MTLType AsMTLType<T>(this T type) 
        where T : struct, INativeHandle<T>, IMTLType =>
        type.UnsafeCast<T, MTLType>();

    internal static MTLDataType GetDataType<T>(this T type) where T : struct, INativeHandle<T>, IMTLType
    {
#pragma warning disable CA1416
        // All subclasses of MTLType are assumed to have a data type property originally
        // before they become subclasses of MTLType.
        
        // This call emits a warning but it can be ignored since the call to get the property value
        // is guaranteed to be safe.
        return type.AsMTLType().DataType;
#pragma warning restore CA1416
    }
}

[SupportedOSPlatform("macos10.13")]
[SupportedOSPlatform("ios11.0")]
public struct MTLType : IObjCInterface<MTLType>, IMTLType
{
    public readonly IntPtr Handle;
    IntPtr INativeHandle<MTLType>.Handle => Handle;
    public MTLType(IntPtr handle) => Handle = handle;

    public MTLDataType DataType
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetDataTypeDelegate>()(Handle, "dataType");
        }
    }
    
    static MTLType INativeHandle<MTLType>.CreateWithHandle(IntPtr handle) => new(handle);
    public static ObjCClass RuntimeClass { get; } = nameof(MTLType);
}