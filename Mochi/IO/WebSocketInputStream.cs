using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using Mochi.Utils;

namespace Mochi.IO;

public class WebSocketInputStream : Stream
{
    private readonly WebSocket _webSocket;

    public WebSocketInputStream(WebSocket webSocket)
    {
        _webSocket = webSocket;
    }
    
    public override void Flush() => throw new NotSupportedException();

    public override int Read(byte[] buffer, int offset, int count)
    {
        var result = _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer, offset, count), CancellationToken.None)
            .Result;
        
        if (result.CloseStatus.HasValue)
        {
            _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None).Wait();
        }
        
        return result.Count;
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    public override bool CanRead => !_webSocket.CloseStatus.HasValue;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();
    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }
}