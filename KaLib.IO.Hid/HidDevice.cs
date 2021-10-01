using System;
using KaLib.IO.Hid.Native;

namespace KaLib.IO.Hid
{
    public class HidDevice : IDisposable
    {
        private HidDeviceInfo _info;
        /* device handle */
        private unsafe NativeHidDevice* handle;

        /* dispose */
        public void Dispose()
        {
            Close();
        }

        /* open hid device */
        public bool Open(HidDeviceInfo dev)
        {
            unsafe
            {
                handle = HidApi.OpenPath(dev.Path);
                return handle != null;
            }
        }

        /* close hid device */
        public void Close()
        {
            unsafe
            {
                HidApi.Close(handle);
            }
        }

        /* write record */
        public void Write(byte[] data)
        {
            unsafe
            {
                HidApi.Write(handle, data, data.Length);
            }
        }

        /* read record */
        public void Read(byte[] data)
        {
            unsafe
            {
                HidApi.Read(handle, data, data.Length);
            }
        }
    }
}