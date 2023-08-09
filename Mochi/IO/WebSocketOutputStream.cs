using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;

namespace Mochi.IO;

public class WebSocketOutputStream : Stream
{
    private readonly WebSocket _webSocket;
    private readonly WebSocketMessageType _type;

    public WebSocketOutputStream(WebSocket webSocket, WebSocketMessageType type)
    {
        _webSocket = webSocket;
        _type = type;
    }

    public override void Flush()
    {
        
    }

    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    
    public override void Write(byte[] buffer, int offset, int count)
    {
        _webSocket.SendAsync(new ArraySegment<byte>(buffer, offset, count), _type, true,
            CancellationToken.None).Wait();
        
        if (_webSocket.CloseStatus.HasValue)
        {
            _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None).Wait();
        }
    }

    public override bool CanRead => false;
    public override bool CanSeek => false;
    public override bool CanWrite => !_webSocket.CloseStatus.HasValue;
    public override long Length => throw new NotSupportedException();
    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }
}