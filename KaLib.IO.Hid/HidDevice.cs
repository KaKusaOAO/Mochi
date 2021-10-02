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
        public int Write(byte[] data)
        {
            unsafe
            {
                int len = HidApi.Write(handle, data, data.Length);
                return len;
                
                Console.Write("Write: ");
                for (int i = 0; i < len; i++)
                {
                    Console.Write($"{data[i]:x2} ");
                }
                Console.WriteLine();
                return len;
            }
        }

        /* read record */
        public int Read(byte[] data)
        {
            unsafe
            {
                int len = HidApi.Read(handle, data, data.Length);
                return len;
                
                Console.Write("Read: ");
                for (int i = 0; i < len; i++)
                {
                    Console.Write($"{data[i]:x2} ");
                }
                Console.WriteLine();
                return len;
            }
        }
        
        public int ReadTimeout(byte[] data, int millis)
        {
            unsafe
            {
                int len = HidApi.ReadTimeout(handle, data, data.Length, millis);
                return len;
                
                Console.Write("ReadTimeout: ");
                for (int i = 0; i < len; i++)
                {
                    Console.Write($"{data[i]:x2} ");
                }
                Console.WriteLine();
                return len;
            }
        }
    }
}