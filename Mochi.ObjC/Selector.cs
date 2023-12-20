using System.Diagnostics;
using System.Text;

namespace Mochi.ObjC;

[DebuggerDisplay("{ToString()}")]
public unsafe struct Selector
{
    public readonly IntPtr Handle;

    public Selector(IntPtr handle)
    {
        Handle = handle;
    }
    
    public Selector(string name)
    {
        var bytes = Encoding.UTF8.GetBytes(name);
        fixed (byte* ptr = bytes)
        {
            Handle = Native.sel_registerName(ptr);
        }
    }

    public override string ToString()
    {
        var bytes = Native.sel_getName(Handle);
        var chars = 0;

        while (bytes[chars] != 0)
        {
            chars++;
        }

        var name = Encoding.UTF8.GetString(bytes, chars);
        return $"0x{Handle:x} @selector({name})";
    }

    public static implicit operator Selector(string s) => new(s);
}