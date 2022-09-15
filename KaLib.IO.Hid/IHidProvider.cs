using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using KaLib.IO.Hid.Platform;
using KaLib.IO.Hid.Platform.MacOs;

namespace KaLib.IO.Hid;

public interface IHidProvider : IDisposable
{
    internal void Init();
    internal HidDevice CreateDevice();
    IEnumerable<HidDeviceInfo> GetDevicesInfo();
    HidDevice OpenDevice(ushort vendorId, ushort productId, string serialNumber);
    HidDevice OpenDevice(string path);
}

public static class HidProviders
{
    private static IHidProvider _provider;

    private static IHidProvider ResolveProvider()
    {
        if (_provider != null) return _provider;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            _provider = new MacOsHidProvider();
        }

        if (_provider == null) throw new PlatformNotSupportedException();
        
        _provider.Init();
        return _provider;
    }

    public static IHidProvider Shared => ResolveProvider();
}