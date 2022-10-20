﻿using System.Runtime.InteropServices;
using KaLib.Utils;

namespace KaLib.IO.Controllers.XInput.Natives;

public static class XInput
{
    private static IXInputProvider _provider;
    
    static XInput()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Logger.Verbose("Using built-in XInput API");
            _provider = new XInputWin32();
            return;
        }

        throw new PlatformNotSupportedException($"XInput is currently not supported on {RuntimeInformation.OSDescription}");
    }

    public static XInputState GetState(PlayerIndex index) => _provider.GetState(index);

    public static void SetState(PlayerIndex index, float leftMotor, float rightMotor) =>
        _provider.SetState(index, leftMotor, rightMotor);
}