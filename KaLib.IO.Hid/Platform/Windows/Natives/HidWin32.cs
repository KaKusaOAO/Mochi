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

    [DllImport(DllName, EntryPoint = "HidD_GetManufacturerString")]
    public static extern bool GetManufacturerString(IntPtr device, IntPtr buffer, ulong bufferLen);

    [DllImport(DllName, EntryPoint = "HidD_GetProductString")]
    public static extern bool GetProductString(IntPtr device, IntPtr buffer, ulong bufferLen);

    [DllImport(DllName, EntryPoint = "HidD_SetFeature")]
    public static extern bool SetFeature(IntPtr device, IntPtr buffer, ulong bufferLen);

    [DllImport(DllName, EntryPoint = "HidD_GetFeature")]
    public static extern bool GetFeature(IntPtr device, IntPtr buffer, ulong bufferLen);

    [DllImport(DllName, EntryPoint = "HidD_GetInputReport")]
    public static extern bool GetInputReport(IntPtr device, IntPtr buffer, ulong bufferLen);
    
    [DllImport(DllName, EntryPoint = "HidD_GetIndexedString")]
    public static extern bool GetIndexedString(IntPtr device, ulong index, IntPtr buffer, ulong bufferLen);

    [DllImport(DllName, EntryPoint = "HidD_GetPreparsedData")]
    public static extern bool GetPreparsedData(IntPtr device, out HidpPreparsedData data);

    [DllImport(DllName, EntryPoint = "HidD_FreePreparsedData")]
    public static extern bool FreePreparsedData(ref HidpPreparsedData data);

    [DllImport(DllName, EntryPoint = "HidD_GetCaps")]
    public static extern uint GetCaps(ref HidpPreparsedData data, out HidpCaps caps);

    [DllImport(DllName, EntryPoint = "HidD_SetNumInputBuffers")]
    public static extern bool SetNumInputBuffers(IntPtr device, ulong bufferCount);
}