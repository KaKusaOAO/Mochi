using System;
using System.Runtime.InteropServices;
using KaLib.IO.Hid.Native;

namespace KaLib.IO.Hid
{
    public class HidDevice : IDisposable
    {
        private HidDeviceInfo _info;
        private unsafe NativeHidDevice* handle;
        public unsafe bool Closed => handle == null;

        public void Dispose()
        {
            Close();
        }

        public bool Open(HidDeviceInfo dev)
        {
            unsafe
            {
                handle = HidApi.OpenPath(dev.Path);
                return handle != null;
            }
        }

        public void Close()
        {
            unsafe
            {
                if(handle != null)
                    HidApi.Close(handle);
            }
        }

        public int Write(byte[] data)
        {
            unsafe
            {
                return HidApi.Write(handle, data, data.Length);
            }
        }

        public int Read(byte[] data)
        {
            unsafe
            {
                return HidApi.Read(handle, data, data.Length);
            }
        }
        
        public int ReadTimeout(byte[] data, int millis)
        {
            unsafe
            {
                return HidApi.ReadTimeout(handle, data, data.Length, millis);
            }
        }

        public string GetLastError()
        {
            unsafe
            {
                return HidApi.Error(handle);
            }
        }
    }
}