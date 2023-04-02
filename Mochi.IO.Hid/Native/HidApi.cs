using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace Mochi.IO.Hid.Native;

internal static unsafe class HidApi
{
    static HidApi()
    {
        if (Environment.OSVersion.Platform != PlatformID.Win32NT) return;
        var path = Path.GetDirectoryName(typeof(HidApi).Assembly.Location) ?? "";
        path = Path.Combine(path, IntPtr.Size == 8 ? "x64" : "x86");
        if (!SetDllDirectory(path)) throw new Win32Exception();
            
            
    }
        
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool SetDllDirectory(string path);
        
    private const string DllName = "hidapi";

    [DllImport(DllName, EntryPoint = "hid_enumerate")]
    public static extern NativeHidDeviceInfo* Enumerate(ushort vendorId = 0, ushort productId = 0);

    [DllImport(DllName, EntryPoint = "hid_free_enumeration")]
    public static extern void FreeEnumeration(NativeHidDeviceInfo* devs);

    [DllImport(DllName, EntryPoint = "hid_open")]
    public static extern NativeHidDevice* Open(ushort vendorId, ushort productId,
        [MarshalAs(UnmanagedType.LPWStr)] string serial = null);

    [DllImport(DllName, EntryPoint = "hid_open_path", CharSet = CharSet.Ansi)]
    public static extern NativeHidDevice* OpenPath(string path);

    [DllImport(DllName, EntryPoint = "hid_write")]
    public static extern int Write(NativeHidDevice* device, byte[] data, int length);

    [DllImport(DllName, EntryPoint = "hid_read_timeout")]
    public static extern int ReadTimeout(NativeHidDevice* device, byte[] data, int length, int milliseconds);

    [DllImport(DllName, EntryPoint = "hid_read")]
    public static extern int Read(NativeHidDevice* device, byte[] data, int length);

    [DllImport(DllName, EntryPoint = "hid_set_nonblocking")]
    public static extern int SetNonBlocking(NativeHidDevice* device, bool nonblock);

    [DllImport(DllName, EntryPoint = "hid_send_feature_report")]
    public static extern int SendFeatureReport(NativeHidDevice* device, byte[] data, int length);

    [DllImport(DllName, EntryPoint = "hid_get_feature_report")]
    public static extern int GetFeatureReport(NativeHidDevice* device, byte[] data, int length);

    [DllImport(DllName, EntryPoint = "hid_close")]
    public static extern void Close(NativeHidDevice* device);

    [DllImport(DllName, EntryPoint = "hid_get_manufacturer_string")]
    public static extern int GetManufacturerString(NativeHidDevice* device,
        [MarshalAs(UnmanagedType.LPWStr)] ref string str, int maxlen);

    [DllImport(DllName, EntryPoint = "hid_get_product_string")]
    public static extern int GetProductString(NativeHidDevice* device,
        [MarshalAs(UnmanagedType.LPWStr)] ref string str, int maxlen);

    [DllImport(DllName, EntryPoint = "hid_get_serial_number_string")]
    public static extern int GetSerialNumberString(NativeHidDevice* device,
        [MarshalAs(UnmanagedType.LPWStr)] ref string str, int maxlen);

    [DllImport(DllName, EntryPoint = "hid_get_indexed_string")]
    public static extern int GetIndexedString(NativeHidDevice* device, int strIndex,
        [MarshalAs(UnmanagedType.LPWStr)] ref string str, int maxlen);

    [DllImport(DllName, EntryPoint = "hid_error")]
    public static extern IntPtr Error(NativeHidDevice* device);

    [DllImport(DllName, EntryPoint = "hid_get_input_report")]
    public static extern int GetInputReport(NativeHidDevice* device, byte[] data, int length);
}
    
internal struct NativeHidDevice
{
    // This structure is platform-dependant,
    // we should leave it empty here
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct NativeHidDeviceInfo
{
    public char* Path;
    public ushort VendorId;
    public ushort ProductId;
    public char* SerialNumber;
    public ushort ReleaseNumber;
    public char* ManufacturerString;
    public char* ProductString;
    public ushort UsagePage;
    public ushort Usage;
    public int InterfaceNumber;
    public NativeHidDeviceInfo* Next;
    public BusType BusType;
}

public enum BusType
{
    Unknown,
    Usb,
    Bluetooth,
    I2C,
    Spi
}