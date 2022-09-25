using System;
using System.Runtime.InteropServices;
using KaLib.IO.Hid.Native;

namespace KaLib.IO.Hid
{
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
                var path = Marshal.PtrToStringAuto((IntPtr)handle.Path);
                var manufacturer = Marshal.PtrToStringAuto((IntPtr)handle.ManufacturerString);
                var product = Marshal.PtrToStringAuto((IntPtr)handle.ProductString);
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
}