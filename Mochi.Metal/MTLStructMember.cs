using Mochi.ObjC;

namespace Mochi.Metal;

public struct MTLStructMember : IObjCInterface<MTLStructMember>
{
    public readonly IntPtr Handle;
    IntPtr INativeHandle<MTLStructMember>.Handle => Handle;
    public MTLStructMember(IntPtr handle) => Handle = handle;

    public NSString Name
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetNSStringDelegate>()(Handle, 
                CommonSelectors.Name);
        }
    }
    
    public NSUInteger Offset
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetNSUIntDelegate>()(Handle, _selOffset);
        }
    }
    
    public MTLDataType DataType
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetDataTypeDelegate>()(Handle, _selDataType);
        }
    }
    
    static MTLStructMember INativeHandle<MTLStructMember>.CreateWithHandle(IntPtr handle) => new(handle);
    public static ObjCClass RuntimeClass { get; } = nameof(MTLStructMember);

    private static readonly Selector _selOffset = "offset";
    private static readonly Selector _selDataType = "dataType";
}