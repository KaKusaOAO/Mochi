using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using KaLib.IO.Hid.Platform.MacOs.Natives;

namespace KaLib.IO.Hid.Platform.MacOs;

internal unsafe class MacOsHidProvider : IHidProvider
{
    private static readonly IntPtr _kCFAllocatorDefault = IntPtr.Zero;
    private static readonly IntPtr _kCFRunLoopDefaultMode = new(0x1f4061650);
    private IOHIDManager* _hidManager;
    private bool _isYosemiteOrNewer;

    void IHidProvider.Init()
    {
        if (_hidManager == null)
        {
            var version = Environment.OSVersion.Version;
            _isYosemiteOrNewer = version.Major > 10 || version.Major == 10 && version.Minor >= 10;
            InitHidManager();
        }
    }

    private void InitHidManager()
    {
        _hidManager = IOKit.IOHIDManagerCreate(_kCFAllocatorDefault, HidOptionsType.None);
        if (_hidManager == null) throw new NotSupportedException();
        
        IOKit.IOHIDManagerSetDeviceMatching(_hidManager, IntPtr.Zero);
        IOKit.IOHIDManagerScheduleWithRunLoop(_hidManager, CoreFoundation.CFRunLoopGetCurrent(), _kCFRunLoopDefaultMode);
    }

    public IEnumerable<HidDeviceInfo> GetDevicesInfo()
    {
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

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}