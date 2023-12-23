using Mochi.ObjC;

namespace Mochi.Metal;

public struct MTLStructType : IObjCInterface<MTLStructType>, IMTLType
{
    public readonly IntPtr Handle;
    IntPtr INativeHandle<MTLStructType>.Handle => Handle;
    public MTLStructType(IntPtr handle) => Handle = handle;

    public MTLDataType DataType => this.GetDataType();

    public NSArray<MTLStructMember> Members
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetNSArrayDelegate>()(Handle, _selMembers)
                .AsTyped<MTLStructMember>();
        }
    }

    public MTLStructMember MemberByName(NSString name)
    {
        this.EnsureInstanceNotNull();
        return ObjCRuntime.GetSendMessageFunction<MemberByNameDelegate>()(Handle, _selMemberByName, name);
    }
    
    private delegate MTLStructMember MemberByNameDelegate(IntPtr handle, Selector sel, NSString str);
    
    static MTLStructType INativeHandle<MTLStructType>.CreateWithHandle(IntPtr handle) => new(handle);
    public static ObjCClass RuntimeClass { get; } = nameof(MTLStructType);

    private static readonly Selector _selMembers = "members";
    private static readonly Selector _selMemberByName = "memberByName:";
}