using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using KaLib.IO.Hid.Platform.Windows.Natives;

namespace KaLib.IO.Hid.Platform.Windows;

internal unsafe class WindowsHidProvider : IHidProvider
{
    public void Dispose()
    {
        
        throw new NotImplementedException();
    }

    void IHidProvider.Init()
    {
    }

    public IEnumerable<HidDeviceInfo> GetDevicesInfo()
    {
        HidWin32.GetHidGuid(out var interfaceGuid);

        ConfigRet cr;
        IntPtr interfaceList;
        
        do
        {
            cr = CfgMgr32.GetDeviceInterfaceListSize(out var len, ref interfaceGuid, null,
                CfgMgr32.GetDeviceInterfaceListPresent);
            if (cr != ConfigRet.Success)
            {
                throw new HidException("Failed to get size of HID device interface list");
            }

            interfaceList = Marshal.AllocHGlobal((int) len * sizeof(ushort));
            cr = CfgMgr32.GetDeviceInterfaceList(ref interfaceGuid, null, interfaceList, len,
                CfgMgr32.GetDeviceInterfaceListPresent);
            if (cr != ConfigRet.Success && cr != ConfigRet.BufferSmall)
            {
                throw new HidException("Failed to get HID device interface list");
            }
        } while (cr == ConfigRet.BufferSmall);

        for (var dInterface = interfaceList; Marshal.ReadInt16(dInterface) != 0; dInterface += WcsLen(dInterface) + 1)
        {
            var attrib = new HiddAttributes();
            var path = Marshal.PtrToStringUTF8(dInterface);
            var handle = InternalOpenDevice(path, false);
            if (handle == new IntPtr(-1))
            {
                continue;
            }

            attrib.Size = (ulong) sizeof(HiddAttributes);
            if (!HidWin32.GetAttributes(handle, out attrib))
            {
                goto cont_close;
            }

            throw new NotImplementedException();
            
            cont_close:
            Kernel32.CloseHandle(handle);
        }
        
        throw new NotImplementedException();
    }

    private HidDeviceInfo InternalGetDeviceInfo(string path, IntPtr handle)
    {
        var attrib = new HiddAttributes();
        var dev = new HidDeviceInfo();
        dev.Path = path;
        attrib.Size = (ulong) sizeof(HiddAttributes);

        if (HidWin32.GetAttributes(handle, out attrib))
        {
            dev.VendorId = attrib.VendorId;
            dev.ProductId = attrib.ProductId;
            dev.ReleaseNumber = attrib.VersionNumber;
        }

        if (HidWin32.GetPreparsedData(handle, out var ppData))
        {
            if (HidWin32.GetCaps(ref ppData, out var caps) == 0x00110000)
            {
                dev.UsagePage = caps.UsagePage;
                dev.Usage = caps.Usage;
            }

            HidWin32.FreePreparsedData(ref ppData);
        }

        var strSize = 256 * sizeof(ushort);
        var str = Marshal.AllocHGlobal(strSize);
        Marshal.WriteInt16(str, 0, 0);
        HidWin32.GetSerialNumberString(handle, str, (ulong) strSize);
        Marshal.WriteInt16(str, strSize - sizeof(ushort), 0);
        dev.SerialNumber = Marshal.PtrToStringUni(str);

        Marshal.WriteInt16(str, 0, 0);
        HidWin32.GetManufacturerString(handle, str, (ulong) strSize);
        Marshal.WriteInt16(str, strSize - sizeof(ushort), 0);
        dev.Manufacturer = Marshal.PtrToStringUni(str);
        
        Marshal.WriteInt16(str, 0, 0);
        HidWin32.GetProductString(handle, str, (ulong) strSize);
        Marshal.WriteInt16(str, strSize - sizeof(ushort), 0);
        dev.Product = Marshal.PtrToStringUni(str);

        InternalGetInfo(path, dev);
        
        return dev;
    }

    private void InternalGetInfo(string path, HidDeviceInfo dev)
    {
        throw new NotImplementedException();
    }

    private static int WcsLen(IntPtr buffer)
    {
        var count = 0;
        while (Marshal.ReadInt16(buffer) != 0)
        {
            buffer += 2;
            count++;
        }

        return count;
    }

    private IntPtr InternalOpenDevice(string path, bool openRw)
    {
        var desiredAccess = openRw ? Kernel32.GenericRead | Kernel32.GenericWrite : 0;
        var sharedMode = 3u; // Read + Write
        return Kernel32.CreateFile(path, desiredAccess, sharedMode, IntPtr.Zero, Kernel32.OpenExisting,
            Kernel32.FileFlagOverlapped, IntPtr.Zero);
    }

    public HidDevice OpenDevice(ushort vendorId, ushort productId, string serialNumber)
    {
        throw new NotImplementedException();
    }

    public HidDevice OpenDevice(string path)
    {
        throw new NotImplementedException();
    }
}