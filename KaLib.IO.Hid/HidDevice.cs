using System;
using System.Runtime.InteropServices;
using KaLib.IO.Hid.Native;

namespace KaLib.IO.Hid
{
    public abstract class HidDevice : IDisposable
    {
        public abstract int Write(byte[] data);
    
        public abstract int Read(byte[] output);
        public abstract int Read(byte[] output, int millis);
        // TODO: Can we use CancellationToken?

        public abstract int SetBlocking(bool blocking);
        public abstract int SendFeatureReport(byte[] data);
        public abstract int GetFeatureReport(byte[] output);
        public abstract int GetInputReport(byte[] output);
        public abstract void Dispose();
        protected abstract string InternalGetManufacturer();
        protected abstract string InternalGetProduct();
        protected abstract string InternalGetSerialNumber();

        public string Manufacturer => InternalGetManufacturer();
        public string Product => InternalGetProduct();
        public string SerialNumber => InternalGetSerialNumber();
    }
}