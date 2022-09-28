using System.Runtime.InteropServices;
using KaLib.Utils;

namespace KaLib.IO.Controllers.XInput.Natives;

public class XInputWin32 : IXInputProvider
{
    private const int ErrDeviceNotConnected = 0x048f;
    private const string Kernel32Lib = "kernel32.dll";
    
    [DllImport(Kernel32Lib, CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr LoadLibraryW(string path);

    [DllImport(Kernel32Lib)]
    private static extern int GetLastError();

    [DllImport(Kernel32Lib)]
    private static extern IntPtr GetProcAddress(IntPtr handle, IntPtr name);
    
    [DllImport(Kernel32Lib)]
    private static extern IntPtr GetProcAddress(IntPtr handle, string name);

    private delegate int XInputGetStateDelegate(int playerIndex, out XInputState state);

    private delegate int XInputSetStateDelegate(int playerIndex, ref XInputVibration state);

    private delegate int XInputGetBatteryInfoDelegate(int playerIndex, byte devType, out XInputBatteryInfo info);

    private static XInputGetStateDelegate _mGetState;
    private static XInputSetStateDelegate _mSetState;
    private static XInputGetBatteryInfoDelegate _mGetBatteryInfo;

    private static IntPtr _handle;
    private static bool _loaded;

    static XInputWin32()
    {
        int x13Err = 0, x14Err, x91Err = 0;
        
        var isXInput14 = true;
        _handle = LoadLibraryW("xinput1_4.dll");
        x14Err = GetLastError();

        if (_handle == IntPtr.Zero)
        {
            isXInput14 = false;
            _handle = LoadLibraryW("xinput1_3.dll");
            x13Err = GetLastError();
        }
        
        if (_handle == IntPtr.Zero)
        {
            _handle = LoadLibraryW("xinput9_1_0.dll");
            x91Err = GetLastError();
        }

        if (_handle == IntPtr.Zero)
        {
            Logger.Warn("Failed to load XInput!");
            Logger.Warn($"XInput 1.4: 0x{x14Err:X8}");
            Logger.Warn($"XInput 1.3: 0x{x13Err:X8}");
            Logger.Warn($"XInput 9.1: 0x{x91Err:X8}");
            return;
        }

        _mGetState = GetProcAddress<XInputGetStateDelegate>(_handle, new IntPtr(100));
        _mSetState = GetProcAddress<XInputSetStateDelegate>(_handle, "XInputSetState");
        if (isXInput14)
        {
            _mGetBatteryInfo = GetProcAddress<XInputGetBatteryInfoDelegate>(_handle, "XInputGetBatteryInformation");
        }

        _loaded = true;
    }

    private static T GetProcAddress<T>(IntPtr handle, IntPtr name)
    {
        var result = GetProcAddress(handle, name);
        return Marshal.GetDelegateForFunctionPointer<T>(result);
    }
    
    private static T GetProcAddress<T>(IntPtr handle, string name)
    {
        var result = GetProcAddress(handle, name);
        return Marshal.GetDelegateForFunctionPointer<T>(result);
    }
    
    public XInputState GetState(PlayerIndex index)
    {
        var result = _mGetState((int) index, out var state);
        if (result == ErrDeviceNotConnected)
        {
            throw new XInputException("The controller is not connected");
        }
        return state;
    }
    
    public void SetState(PlayerIndex index, float leftMotor, float rightMotor)
    {
        var state = new XInputVibration
        {
            LeftRumble = (short) (leftMotor * short.MaxValue),
            RightRumble = (short) (rightMotor * short.MaxValue),
        };
        
        var result = _mSetState((int) index, ref state);
        if (result == ErrDeviceNotConnected)
        {
            throw new XInputException("The controller is not connected");
        }
    }
}