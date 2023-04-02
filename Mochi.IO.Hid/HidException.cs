using System.IO;
using Mochi.IO.Hid.Native;
using Mochi.Utils;

namespace Mochi.IO.Hid;

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