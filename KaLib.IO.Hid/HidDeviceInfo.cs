using System;
using System.Runtime.InteropServices;
using KaLib.IO.Hid.Native;
using KaLib.Utils;

namespace KaLib.IO.Hid;

public class HidDeviceInfo
{
    /* device path */
    public string Path { get; internal set; }
        
    /* vendor ID */
    public ushort VendorId { get; internal set; }
        
    /* product id */
    public ushort ProductId { get; internal set; }
        
    /* usb serial number string */
    public string SerialNumber { get; internal set; }
    public ushort ReleaseNumber { get; internal set; }
        
    /* usb manufacturer string */
    public string Manufacturer { get; internal set; }
        
    /* usb product string */
    public string Product { get; internal set; }
    public ushort UsagePage { get; internal set; }
    public ushort Usage { get; internal set; }
        
    public BusType BusType { get; internal set; }

    internal HidDeviceInfo()
    {
    }
}