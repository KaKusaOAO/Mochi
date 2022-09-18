using System;
using System.IO;
using System.Runtime.InteropServices;
using KaLib.IO.Hid.Native;

namespace KaLib.IO.Hid;

public class HidException : IOException
{
    public HidException(string message) : base(message) {}

    public static HidException CreateFromLast(HidDevice device)
    {
        unsafe
        {
            var message = HidApi.Error(device.handle);
            var msg = Marshal.PtrToStringAuto(message);
            return new HidException(msg);
        }
    }
        
}