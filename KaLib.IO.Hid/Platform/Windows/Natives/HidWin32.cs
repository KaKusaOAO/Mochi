using System;
using System.Runtime.InteropServices;

namespace KaLib.IO.Hid.Platform.Windows.Natives;

internal static unsafe class HidWin32
{
    private const string DllName = "hid.dll";

    [DllImport(DllName, EntryPoint = "HidD_GetHidGuid")]
    public static extern void GetHidGuid(out Guid result);

    [DllImport(DllName, EntryPoint = "HidD_GetAttributes")]
    public static extern bool GetAttributes(IntPtr device, out HiddAttributes result);

    [DllImport(DllName, EntryPoint = "HidD_GetSerialNumberString")]
    public static extern bool GetSerialNumberString(IntPtr device, IntPtr buffer, ulong bufferLen);

}