using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mochi.ObjC;

public static class ObjCRuntime
{
    public static T GetSendMessageFunction<T>()
    {
        if (!Native.IsSupported)
            throw new PlatformNotSupportedException("Trying to use Objective-C on an unsupported platform!");
        
        return Marshal.GetDelegateForFunctionPointer<T>(Native.MsgSendPointer);
    }
}