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

        public unsafe void SetNonBlocking(bool value)
        {
            if (Closed) return;
            Wrap(() => HidApi.SetNonBlocking(handle, value));
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
            if (Closed) return;
            unsafe
            {
                if (handle != null)
                {
                    HidApi.Close(handle);
                    handle = null;
                }
            }
        }

        public int Write(byte[] data)
        {
            if (Closed) return -1;
            unsafe
            { 
                return Wrap(() => HidApi.Write(handle, data, data.Length));
            }
        }

        public int Read(byte[] data)
        {
            if (Closed) return -1;
            unsafe
            {
                return Wrap(() => HidApi.Read(handle, data, data.Length));
            }
        }

        public int FlushAndRead(byte[] data)
        {
            if (Closed) return -1;
            var trunk = new byte[data.Length];
            var hasRead = false;
            while (true)
            {
                // Read the newest data, or loop if no data is available yet
                var read = Read(trunk);
                if (read > 0) hasRead = true;
                if (read == 0 && hasRead)
                {
                    Array.Copy(trunk, data, data.Length);
                    return read;
                }
            }
        }

        public int GetInputReport(byte[] data)
        {
            if (Closed) return -1;
            unsafe
            {
                return Wrap(() => HidApi.GetInputReport(handle, data, data.Length));
            }
        }
        
        public int ReadTimeout(byte[] data, int millis)
        {
            if (Closed) return -1;
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