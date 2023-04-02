using System;
using System.Runtime.InteropServices;
using Mochi.IO.Hid.Native;
using Mochi.Utils;

namespace Mochi.IO.Hid;

public class HidDeviceInfo
{
    /* device path */
    public string Path { get; }
        
    /* vendor ID */
    public ushort VendorId { get; }
        
    /* product id */
    public ushort ProductId { get; }
        
    /* usb product string */
    public string Product { get; }
        
    /* usb manufacturer string */
    public string Manufacturer { get; }
        
    /* usb serial number string */
    public string SerialNumber { get; }
        
    internal NativeHidDeviceInfo Handle { get; }
        
    public BusType BusType { get; }

    internal HidDeviceInfo(NativeHidDeviceInfo handle)
    {
        unsafe
        {
            var path = Marshal.PtrToStringAnsi((IntPtr)handle.Path);
            var manufacturer = Common.GetNullTerminatedWideString((IntPtr)handle.ManufacturerString);
            var product = Common.GetNullTerminatedWideString((IntPtr)handle.ProductString);
            var vendorId = handle.VendorId;
            var productId = handle.ProductId;
            var serial = new string(handle.SerialNumber);

            Handle = handle;
            Product = product;
            SerialNumber = serial;
            Manufacturer = manufacturer;
            Path = path;
            VendorId = vendorId;
            ProductId = productId;
            BusType = handle.BusType;
        }
    }
}