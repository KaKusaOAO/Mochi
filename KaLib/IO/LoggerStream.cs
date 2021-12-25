using System;
using System.IO;
using KaLib.Texts;
using KaLib.Utils;
using KaLib.Utils.Extensions;

namespace KaLib.IO;

public class LoggerStream : Stream
{
    public Stream Underlying { get; }
    public string Name { get; }

    public LoggerStream(Stream stream, string name = "Stream")
    {
        Underlying = stream;
        Name = name;
    }

    public override void Close() => Underlying.Close();

    public override void Flush() => Underlying.Flush();

    public override int Read(byte[] buffer, int offset, int count)
    {
        var len = Underlying.Read(buffer, offset, count);
        if (len >= 0)
        {
            var buf = new byte[len];
            Array.Copy(buffer, offset, buf, 0, len);
            Logger.Log(
                TranslateText.Of($"%s [{Name}]: %s")
                    .SetColor(TextColor.Aqua)
                    .AddWith(LiteralText.Of("<-").SetColor(TextColor.Green))
                    .AddWith(LiteralText.Of(buf.Hexdump()).SetColor(TextColor.Gold)), Underlying.GetType().FullName);
        }
        return len;
    }

    public override long Seek(long offset, SeekOrigin origin)
        => Underlying.Seek(offset, origin);

    public override void SetLength(long value)
        => Underlying.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count)
    {
        Underlying.Write(buffer, offset, count);
        var buf = new byte[count];
        Array.Copy(buffer, offset, buf, 0, count);
        Logger.Log(
            TranslateText.Of($"%s [{Name}]: %s")
                .SetColor(TextColor.Aqua)
                .AddWith(LiteralText.Of("->").SetColor(TextColor.Red))
                .AddWith(LiteralText.Of(buf.Hexdump()).SetColor(TextColor.Gold)), Underlying.GetType().FullName);
    }

    public override bool CanRead => Underlying.CanRead;
    public override bool CanSeek => Underlying.CanSeek;
    public override bool CanWrite => Underlying.CanWrite;
    public override long Length => Underlying.Length;

    public override int ReadTimeout
    {
        get => Underlying.ReadTimeout;
        set => Underlying.ReadTimeout = value;
    }

    public override long Position
    {
        get => Underlying.Position;
        set => Underlying.Position = value;
    }
}