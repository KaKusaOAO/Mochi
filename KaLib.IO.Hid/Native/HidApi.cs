using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace KaLib.IO.Hid.Native
{
    internal static unsafe class HidApi
    {
        static HidApi()
        {
            var path = Path.GetDirectoryName(typeof(HidApi).Assembly.Location);
            path = Path.Combine(path, IntPtr.Size == 8 ? "x64" : "x86");
            if(!SetDllDirectory(path)) throw new Win32Exception();
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
        [return: MarshalAs(UnmanagedType.LPWStr)]
        public static extern string Error(NativeHidDevice* device);
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeHidDevice
    {
        public IntPtr device_handle;
        public bool blocking;
        public ushort output_record_length;
        public int input_record_length;
        public IntPtr last_error_str;
        public int last_error_num;
        public bool read_pending;
        public IntPtr read_buf;
        public NativeOverlapped ol;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct NativeHidDeviceInfo
    {
        public char* path;
        public ushort vendor_id;
        public ushort product_id;
        public char* serial_number;
        public ushort release_number;
        public char* manufacturer_string;
        public char* product_string;
        public ushort usage_page;
        public ushort usage;
        public int interface_number;
        public NativeHidDeviceInfo* next;
    }
}