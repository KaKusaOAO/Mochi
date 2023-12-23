using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mochi.ObjC;

[DebuggerDisplay("{ToString()}")]
public unsafe struct Selector
{
    public readonly IntPtr Handle;

    public string Name
    {
        get
        {
            var bytes = Native.sel_getName(Handle);
            var chars = 0;

            while (bytes[chars] != 0)
            {
                chars++;
            }

            return Encoding.UTF8.GetString(bytes, chars);
        }
    }

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

    public override string ToString() => $"0x{Handle:x} @selector({Name})";

    public static implicit operator Selector(string s) => SelectorCache.Get(s);

    static Selector()
    {
        if (sizeof(Selector) != sizeof(IntPtr))
            throw new Exception("Size of Selector must match IntPtr.");
    }
}