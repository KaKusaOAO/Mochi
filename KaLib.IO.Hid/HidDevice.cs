using System;
using System.Runtime.InteropServices;
using KaLib.IO.Hid.Native;

namespace KaLib.IO.Hid
{
    public class HidDevice : IDisposable
    {
        public HidDeviceInfo Info { get; private set; }
        internal unsafe NativeHidDevice* handle;
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
                var result = handle != null;
                
                if (result)
                {
                    Info = dev;
                }

                return result;
            }
        }

        public void Close()
        {
            unsafe
            {
                if (handle != null)
                {
                    HidApi.Close(handle);
                }
            }
        }

        public int Write(byte[] data)
        {
            unsafe
            { 
                return Wrap(() => HidApi.Write(handle, data, data.Length));
            }
        }

        public int Read(byte[] data)
        {
            unsafe
            {
                return Wrap(() => HidApi.Read(handle, data, data.Length));
            }
        }
        
        public int ReadTimeout(byte[] data, int millis)
        {
            unsafe
            {
                return Wrap(() => HidApi.ReadTimeout(handle, data, data.Length, millis));
            }
        }

        private int Wrap(Func<int> result)
        {
            var r = result();
            if (r < 0)
            {
                throw HidException.CreateFromLast(this);
            }

            return r;
        }
    }
}