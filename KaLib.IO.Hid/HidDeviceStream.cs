using System;
using System.IO;
using KaLib.IO.Hid.Native;

namespace KaLib.IO.Hid;

public class HidDeviceStream : Stream
{
    public HidDevice Device { get; }
    
    public HidDeviceStream(HidDevice device)
    {
        Device = device;
    }

    public override void Close()
    {
        Device.Dispose();
    }

    public override void Flush() {}

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
        // var buf = new byte[count];
        // var len = ReadTimeout == 0 ? Device.Read(buf) : Device.ReadTimeout(buf, ReadTimeout);
        // Array.Copy(buf, 0, buffer, offset, count);
        // return len;
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count)
    {
        var buf = new byte[count];
        Array.Copy(buffer, offset, buf, 0, count);
        if (Device.Write(buf) < 0) throw HidException.CreateFromLast(Device);
    }

    public override bool CanTimeout => true;
    public override bool CanRead { get; }
    public override bool CanSeek { get; }
    public override bool CanWrite { get; }
    public override long Length => throw new NotSupportedException();
    public override int ReadTimeout { get; set; }

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }
}