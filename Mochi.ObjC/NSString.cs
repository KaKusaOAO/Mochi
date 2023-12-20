using System.Diagnostics;
using System.Text;

namespace Mochi.ObjC;

[DebuggerDisplay("{ToString()}")]
public unsafe struct NSString : IObjCInterface<NSString>
{
    public readonly IntPtr Handle;
    IntPtr INativeHandle<NSString>.Handle => Handle;

    public NSString(IntPtr handle)
    {
        Handle = handle;
    }

    static NSString INativeHandle<NSString>.CreateWithHandle(IntPtr handle) => new(handle);

    public NSString(string s)
    {
        var ptr = RuntimeClass.Alloc<IntPtr>();

        fixed (char* utf16Ptr = s)
        {
            var length = (UIntPtr)s.Length;
            var init = ObjCRuntime.GetSendMessageFunction<InitWithCharactersLengthDelegate>();
            ptr = init(ptr, _selInitWithCharacters, utf16Ptr, length);
            Handle = ptr;
        }
    }

    public uint Length
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<LengthDelegate>()(Handle, _selLength);
        }
    }

    public override string ToString()
    {
        if (this.IsNull()) return "<nil>";
        
        var ptr = (byte*) ObjCRuntime.GetSendMessageFunction<Utf8StringDelegate>()(Handle, _selUtf8String);

        var chars = 0;
        while (ptr[chars] != 0)
        {
            chars++;
        }

        return Encoding.UTF8.GetString(ptr, chars);
    }

    private delegate IntPtr InitWithCharactersLengthDelegate(IntPtr obj, Selector sel, char* utf16Ptr, UIntPtr length);
    private delegate IntPtr Utf8StringDelegate(IntPtr obj, Selector sel);
    private delegate uint LengthDelegate(IntPtr obj, Selector sel);

    public static implicit operator string(NSString nss) => nss.ToString();
    public static implicit operator NSString(string s) => new(s);
    
    public static ObjCClass RuntimeClass { get; } = nameof(NSString);
    private static Selector _selInitWithCharacters = "initWithCharacters:length:";
    private static Selector _selLength = "length";
    private static Selector _selUtf8String = "UTF8String";
}