using System;
using System.Collections.Generic;
using KaLib.IO.Hid.Platform.Windows.Natives;

namespace KaLib.IO.Hid.Platform.Windows;

internal unsafe class WindowsHidProvider : IHidProvider
{
    public void Dispose()
    {
        
        throw new NotImplementedException();
    }

    void IHidProvider.Init()
    {
    }

    HidDevice IHidProvider.CreateDevice()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<HidDeviceInfo> GetDevicesInfo()
    {
        HidWin32.GetHidGuid(out var interfaceGuid);
        
        throw new NotImplementedException();
    }

    public HidDevice OpenDevice(ushort vendorId, ushort productId, string serialNumber)
    {
        throw new NotImplementedException();
    }

    public HidDevice OpenDevice(string path)
    {
        throw new NotImplementedException();
    }
}