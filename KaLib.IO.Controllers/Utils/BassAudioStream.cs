using System.Collections.Concurrent;
using KaLib.Utils;
using ManagedBass;
using ManagedBass.Fx;

namespace KaLib.IO.Controllers.IO;

public class BassAudioStream : Stream
{
    private const int Frequency = 48000;

    private readonly byte[] _audioBuffer = new byte[Frequency * 4];
    private readonly int _fx;
    private readonly int _stream;
    private readonly int _device;
    private int _audioBufferIdx;

    private bool _hasWrittenOnce;

    private float _playbackPitch;

    private float _playbackRate = 1;

    private Thread _thread;
    private ConcurrentQueue<Action> _taskQueue = new();

    static BassAudioStream()
    {
        if (!Bass.Init(Frequency: 48000))
        {
            var err = Bass.LastError;
            if (err != Errors.Already) throw new BassException(Bass.LastError);
        }

        Logger.Verbose($"Bass initialized ({Bass.Version})");
    }

    public void StartPlayback()
    {
        if (!Bass.ChannelPlay(_fx)) throw new BassException();
    }
    
    public BassAudioStream(int device = -1, int channels = 2)
    {
        _device = device;
        Bass.Init(device, 48000);
        _stream = Bass.CreateStream(Frequency, channels, BassFlags.Decode, StreamProcedureType.Push);

        if (_stream == 0)
        {
            var err = Bass.LastError;
            Logger.Error($"Error occurred while creating stream: {err}");
            throw new BassException(err);
        }

        _fx = BassFx.TempoCreate(_stream, BassFlags.FxFreeSource);
        if (_fx == 0)
        {
            var err = Bass.LastError;
            Logger.Error($"Error occurred while creating FX: {err}");
            throw new BassException(err);
        }
    }

    private void RunEventLoop()
    {
        
    }

    public bool IsClosed { get; private set; }

    public float PlaybackRate
    {
        get => _playbackRate;
        set => SetPlaybackRate(value);
    }

    public float PlaybackPitch
    {
        get => _playbackPitch;
        set => SetPlaybackPitch(value);
    }

    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool CanWrite => true;
    public override long Length => Bass.ChannelGetLength(_stream);

    public override long Position
    {
        get => Bass.ChannelGetPosition(_stream);
        set => Bass.ChannelSetPosition(_stream, value);
    }

    public float PlaybackTime => (float) Bass.ChannelBytes2Seconds(_fx, Position);

    public event Func<Task> Closed;

    public void SetPlaybackRate(float rate, bool sync = false)
    {
        Bass.ChannelSetAttribute(_fx, ChannelAttribute.Tempo, (rate - 1) * 100);
        _playbackRate = rate;

        if (sync)
        {
            var semitones = MathF.Log(rate, 2) * 12;
            Bass.ChannelSetAttribute(_fx, ChannelAttribute.Pitch, semitones);
            _playbackPitch = semitones;
        }
    }

    public void SetPlaybackPitch(float semitones, bool sync = false)
    {
        Bass.ChannelSetAttribute(_fx, ChannelAttribute.Pitch, semitones);
        _playbackPitch = semitones;

        if (sync)
        {
            var rate = MathF.Pow(2, semitones / 12);
            Bass.ChannelSetAttribute(_fx, ChannelAttribute.Tempo, (rate - 1) * 100);
            _playbackRate = rate;
        }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        var buf = new byte[count];
        Array.Clear(buffer);

        var result = Bass.ChannelGetData(_fx, buf, count);
        if (result == -1)
        {
            if (Bass.LastError == Errors.Ended) return 0;
            throw new BassException(Bass.LastError);
        }

        if (result == 0 && !_hasWrittenOnce) result = count;

        Array.Copy(buf, 0, buffer, offset, result);
        return result;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
        // throw new System.NotImplementedException();
    }

    private void InternalWrite(byte[] buffer, int offset, int count)
    {
        const int freq = Frequency * 4;
        if (_audioBufferIdx + count >= freq)
        {
            var remain = freq - _audioBufferIdx;
            Array.Copy(buffer, offset, _audioBuffer, _audioBufferIdx, remain);

            if (Bass.StreamPutData(_stream, _audioBuffer, freq) == -1)
            {
                var err = Bass.LastError;
                if (err == Errors.Handle) throw new ObjectDisposedException("_stream");
                if (err != Errors.Ended) Logger.Error($"Error occurred while writing to channel: {Bass.LastError}");
            }

            _hasWrittenOnce = true;

            Array.Clear(_audioBuffer);
            Array.Copy(buffer, offset + remain, _audioBuffer, 0, count - remain);
            _audioBufferIdx = 0;
        }
        else
        {
            Array.Copy(buffer, offset, _audioBuffer, _audioBufferIdx, count);
            _audioBufferIdx += count;
        }
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        const int freq = Frequency * 4;
        if (count < freq)
        {
            InternalWrite(buffer, offset, count);
            return;
        }

        for (var i = 0; i < count; i += freq) InternalWrite(buffer, offset + i, freq);
    }

    public override void Flush()
    {
        if (Bass.StreamPutData(_stream, _audioBuffer, _audioBuffer.Length | (int) StreamProcedureType.End) == -1)
        {
            var err = Bass.LastError;
            if (err != Errors.Ended) Logger.Error($"Error occurred while flushing: {Bass.LastError}");
        }

        _audioBufferIdx = 0;
        _hasWrittenOnce = true;
    }

    public override void Close()
    {
        base.Close();
        Bass.CurrentDevice = _device;
        Bass.StreamFree(_fx);
        Bass.StreamFree(_stream);
        IsClosed = true;
        Closed?.Invoke();
    }
}