using System;
using System.Runtime.InteropServices;
using System.Text;

namespace KaLib.IO.Hid.Platform.Windows.Natives;

[StructLayout(LayoutKind.Sequential)]
internal struct DeviceInstance
{
    public uint Handle;
}

[StructLayout(LayoutKind.Sequential)]
internal struct DevicePropKey
{
    public Guid Category;
    public ulong Id;
}

internal static class CfgMgr32
{
    private const string DllName = "cfgmgr32.dll";

    public const uint GetDeviceInterfaceListPresent = 0;

    [DllImport(DllName, EntryPoint = "CM_Locate_DevNodeW")]
    public static extern ConfigRet LocateDevNode(out DeviceInstance result,
        [MarshalAs(UnmanagedType.LPWStr)] string deviceId, ulong flags);

    [DllImport(DllName, EntryPoint = "CM_Get_Parent")]
    public static extern ConfigRet GetParent(out DeviceInstance result, DeviceInstance instance, ulong flags);
    
    [DllImport(DllName, EntryPoint = "CM_Get_DevNode_PropertyW")]
    public static extern ConfigRet GetDevNodeProperty(DeviceInstance instance, ref DevicePropKey propKey,
        out ulong propType, out IntPtr buffer, ref ulong bufferSize, ulong flags);

    [DllImport(DllName, EntryPoint = "CM_Get_Device_Interface_PropertyW")]
    public static extern ConfigRet GetDeviceInterfaceProperty(string deviceInterface, ref DevicePropKey propKey,
        out ulong propType, out IntPtr buffer, ref ulong bufferSize, ulong flags);

    [DllImport(DllName, EntryPoint = "CM_Get_Device_Interface_List_SizeW")]
    public static extern ConfigRet GetDeviceInterfaceListSize(out ulong length, ref Guid guid,
        [MarshalAs(UnmanagedType.LPWStr)] string deviceId, ulong flags);

    [DllImport(DllName, EntryPoint = "CM_Get_Device_Interface_ListW")]
    public static extern ConfigRet GetDeviceInterfaceList(ref Guid guid,
        [MarshalAs(UnmanagedType.LPWStr)] string deviceId, IntPtr buffer,
        ulong bufferLen, ulong flags);
}