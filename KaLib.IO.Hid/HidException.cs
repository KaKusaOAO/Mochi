using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using KaLib.IO.Hid.Native;
using KaLib.Utils;

namespace KaLib.IO.Hid;

public class HidException : IOException
{
    public HidException(string message) : base(message) {}

    public static HidException CreateFromLast(HidDevice device)
    {
        unsafe
        {
            var message = HidApi.Error(device.handle);
            return new HidException(Common.GetNullTerminatedWideString(message));
        }
    }
        
}