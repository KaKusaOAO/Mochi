using System;
using System.Runtime.InteropServices;

namespace KaLib.IO.Hid.Platform.Windows.Natives;

public static class Kernel32
{
    private const string DllName = "kernel32.dll";

    public const uint GenericWrite = 0x40000000;
    public const uint GenericRead = 0x80000000;
    public const uint OpenExisting = 3;
    public const uint FileFlagOverlapped = 0x40000000;
    
    [DllImport(DllName, EntryPoint = "CreateFileW")]
    public static extern IntPtr CreateFile([MarshalAs(UnmanagedType.LPWStr)] string path, uint desiredAccess,
        uint shareMode, ref SecurityAttributes securityAttributes, uint creationDisposition, uint flags,
        IntPtr template);
    
    [DllImport(DllName, EntryPoint = "CreateFileW")]
    public static extern IntPtr CreateFile([MarshalAs(UnmanagedType.LPWStr)] string path, uint desiredAccess,
        uint shareMode, IntPtr securityAttributes, uint creationDisposition, uint flags,
        IntPtr template);

    [DllImport(DllName)]
    public static extern IntPtr CloseHandle(IntPtr handle);
}