using System;
using System.IO;
using KaLib.IO.Hid.Native;

namespace KaLib.IO.Hid;

public class HidException : IOException
{
    public HidException(string message) : base(message) {}

    public static HidException CreateFromLast(HidDevice device)
    {
        throw new NotImplementedException();
    }
        
}