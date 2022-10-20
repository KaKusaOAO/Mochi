using System.Numerics;
using System.Runtime.InteropServices;
using KaLib.IO.Hid;
using KaLib.IO.Hid.Native;
using KaLib.Utils;
using ManagedBass;

namespace KaLib.IO.Controllers.DualSense;

public class DualSenseController : IController<DualSenseSnapshot>, IHybridController, ITwoSideRumbleController<DualSenseRumble>
{
    public delegate void GenerateAudioHapticsDelegate(float[] leftRumble, float[] rightRumble);
    
    private readonly HidDevice _device;
    private readonly ButtonDescription[] _buttons;
    public ConnectionType ConnectionType { get; }

    public GenericStick LeftStick { get; } = new("L3");
    public GenericStick RightStick { get; } = new("R3");
    public DualSenseTrigger LeftTrigger { get; } = new();
    public DualSenseTrigger RightTrigger { get; } = new();
    public GeneralControllerButton ButtonCross { get; } = new("Cross");
    public GeneralControllerButton ButtonCircle { get; } = new("Circle");
    public GeneralControllerButton ButtonSquare { get; } = new("Square");
    public GeneralControllerButton ButtonTriangle { get; } = new("Triangle");
    public GeneralControllerButton LeftShoulder { get; } = new("L1");
    public GeneralControllerButton RightShoulder { get; } = new("R1");
    public GenericDPad DPad { get; } = new();
    public GeneralControllerButton Share { get; } = new();
    public GeneralControllerButton Options { get; } = new();
    public DualSenseTouchPad TouchPad { get; } = new();
    public GeneralControllerButton PlayStationLogo { get; } = new();
    public DualSenseMicButton Mic { get; } = new();
    
    // TODO: The accelerometer values should be in G. Seems to need calibration
    public Vector3 Accelerometer { get; private set; }
    
    // TODO: The gyroscope values should be in deg/s. Seems to need calibration
    public Vector3 Gyroscope { get; private set; }

    public DualSenseRumble LeftRumble { get; } = new()
    {
        Frequency = 110
    };

    public DualSenseRumble RightRumble { get; } = new()
    {
        Frequency = 55
    };

    private const int AccResolutionPerG = 8192;
    private const int AccRange = 4 * AccResolutionPerG;

    private const int GyroResolutionPerDegS = 1024;
    private const int GyroRange = 2048 * GyroResolutionPerDegS;

    public event Action<IControllerButton> ButtonPressed;
    public event Action<IControllerButton> ButtonReleased;
    public event Action<DualSenseSnapshot, DualSenseSnapshot> SnapshotUpdated;
    public event GenerateAudioHapticsDelegate AudioHapticsDataRequested;
    public event Action Disconnected;

    private byte _sequenceTag;
    private int _audioDeviceId = -1;
    private int _audioStream;
    private bool _disposed;

    public bool Disposed => _disposed;
    public bool IsAudioHapticsAvailable { get; private set; }

    private RumbleEmulator _rumbleEmulator = new();

    private float[] _leftChanArr = new float[16380];
    private float[] _rightChanArr = new float[16380];

    public ushort AudioHapticsSampleLength
    {
        get => (ushort) _leftChanArr.Length;
        set
        {
            _leftChanArr = new float[value];
            _rightChanArr = new float[value];
        }
    }
    
    public float[] GetLastLeftChannelSamples()
    {
        var arr = new float[_leftChanArr.Length];
        Array.Copy(_leftChanArr, arr, arr.Length);
        return arr;
    }
    
    public float[] GetLastRightChannelSamples()
    {
        var arr = new float[_rightChanArr.Length];
        Array.Copy(_rightChanArr, arr, arr.Length);
        return arr;
    }
    
    /// <summary>
    /// If enabled, the default rumble emulation will be disabled, and the audio data will be pulled from <see cref="AudioHapticsDataRequested"/>.
    /// </summary>
    public bool EnableAudioHaptics { get; set; }
    
    public DualSenseController(HidDevice device)
    {
        _device = device;
        
        ConnectionType = device.Info.BusType == BusType.Bluetooth ? ConnectionType.Bluetooth : ConnectionType.Usb;

        _buttons = new ButtonDescription[]
        {
            new(LeftStick, s => s.LeftStick.Pressed),
            new(RightStick, s => s.RightStick.Pressed),
            new(LeftTrigger, s => s.IsLeftTriggerPressed()),
            new(RightTrigger, s => s.IsRightTriggerPressed()),
            new(ButtonCircle, s => s.ButtonCircle), 
            new(ButtonCross, s => s.ButtonCross),
            new(ButtonSquare, s => s.ButtonSquare),
            new(ButtonTriangle, s => s.ButtonTriangle),
            new(LeftShoulder, s => s.LeftShoulder),
            new(RightShoulder, s => s.RightShoulder),
            new(DPad.Up, s => s.DPad.Up),
            new(DPad.Down, s => s.DPad.Down),
            new(DPad.Left, s => s.DPad.Left),
            new(DPad.Right, s => s.DPad.Right),
            new(Share, s => s.Share),
            new(Options, s => s.Options),
            new(TouchPad, s => s.TouchPad.Pressed),
            new(PlayStationLogo, s => s.PlayStationLogo),
            new(Mic, s => s.Mic)
        };
    }

    public IOptional<DualSenseSnapshot> LastSnapshot { get; private set; } = Optional.Empty<DualSenseSnapshot>();

    public void Initialize()
    {
        _device.SetNonBlocking(true);
        
        Task.Run(async () =>
        {
            var deviceCount = Bass.DeviceCount;
            for (var i = 0; i <= deviceCount; i++)
            {
                // TODO: Well I think there's a better way to do this
                var info = Bass.GetDeviceInfo(i);
                if (!info.Name.Contains("Wireless Controller")) continue;
                _audioDeviceId = i;
                break;
            }
        
            if (_audioDeviceId != -1)
            {
                do
                {
                    if (_disposed) return;
                    if (!Bass.Init(_audioDeviceId, 48000))
                    {
                        var err = Bass.LastError;
                        if (err != Errors.Already)
                        {
                            await Task.Delay(100);
                            continue;
                        }
                    }

                    Bass.PlaybackBufferLength = 6;
                    Bass.UpdatePeriod = 0;
                    
                    _audioStream = Bass.CreateStream(48000, 2, BassFlags.SpeakerRear, (_, buffer, length, _) =>
                    {
                        var len = length / 2;
                        var sample = new short[len];
                        var leftChannel = new float[len / 2];
                        var rightChannel = new float[len / 2];
                        
                        if (EnableAudioHaptics)
                        {
                            AudioHapticsDataRequested?.Invoke(leftChannel, rightChannel);
                        }
                        else
                        {
                            _rumbleEmulator.Emulate(leftChannel, rightChannel, LeftRumble.Frequency,
                                LeftRumble.Amplitude, RightRumble.Frequency, RightRumble.Amplitude);
                        }

                        for (var i = 0; i < len / 2; i++)
                        {
                            sample[i * 2 + 0] = (short) (Math.Clamp(leftChannel[i], -1, 1) * short.MaxValue);
                            sample[i * 2 + 1] = (short) (Math.Clamp(rightChannel[i], -1, 1) * short.MaxValue);
                        }
                        
                        void AppendArray(float[] original, float[] append)
                        {
                            var amount = append.Length;
                            if (amount >= original.Length)
                            {
                                Array.Copy(append, amount - original.Length, original, 0, original.Length);
                                return;
                            }

                            var tmp = new float[original.Length - amount];
                            Array.Copy(original, original.Length - tmp.Length, tmp, 0, tmp.Length);
                            Array.Copy(tmp, original, tmp.Length);
                            Array.Copy(append, 0, original, tmp.Length, amount);
                        }
                        
                        AppendArray(_leftChanArr, leftChannel);
                        AppendArray(_rightChanArr, rightChannel);

                        Marshal.Copy(sample, 0, buffer, sample.Length);
                        return len * 2;
                    });

                    if (_audioStream == 0)
                    {
                        await Task.Delay(100);
                    }
                } while (_audioStream == 0);
            
                Bass.ChannelPlay(_audioStream);

                var audioThread = new Thread(() =>
                {
                    while (!Disposed)
                    {
                        Bass.Update(5);
                        Thread.Sleep(2);
                    }
                })
                {
                    Name = "DualSenseAudio",
                    Priority = ThreadPriority.Highest
                };
                audioThread.Start();
                IsAudioHapticsAvailable = true;
            }
        });
    }

    public override int GetHashCode()
    {
        return _device.Info.Path.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not DualSenseController c) return false;
        return c.GetHashCode() == GetHashCode();
    }

    private record ButtonDescription(IControllerButton Button, Func<DualSenseSnapshot, bool> StateFromSnapshot) { }
    
    public unsafe DualSenseSnapshot PollInput()
    {
        var size = ConnectionType == ConnectionType.Usb ? 64 : 78;
        var buf = new byte[size];
        try
        {
            _device.FlushAndRead(buf);
        }
        catch (HidException)
        {
            Disconnected?.Invoke();
            return default;
        }

        if (ConnectionType == ConnectionType.Bluetooth)
        {
            var report = new byte[size - 4];
            Array.Copy(buf, report, report.Length);

            var inputCrc = BitConverter.ToUInt32(buf, report.Length);
            var expected = DoCrc32(report, 0xa1);
            if (inputCrc != expected)
            {
                throw new Exception("DualSense input CRC check failed");
            }
        }
        
        var packet = Common.BufferToStructure<DualSenseInputState>(buf, ConnectionType == ConnectionType.Usb ? 1 : 2);

        var x = packet.LeftStickX / 128f - 1;
        var y = packet.LeftStickY / 128f - 1;
        LeftStick.Vector = new Vector2(x, -y);

        x = packet.RightStickX / 128f - 1;
        y = packet.RightStickY / 128f - 1;
        RightStick.Vector = new Vector2(x, -y);

        LeftTrigger.Value = packet.LeftTrigger / 255f;
        RightTrigger.Value = packet.RightTrigger / 255f;

        var btnARaw = packet.ButtonsA;
        var btnDPad = btnARaw & 0xf;
        var btnA = (DualSenseButtonsA) (btnARaw & 0xf0);

        DPad.Up.Pressed = btnDPad is 7 or 0 or 1;
        DPad.Right.Pressed = btnDPad is 1 or 2 or 3;
        DPad.Down.Pressed = btnDPad is 3 or 4 or 5;
        DPad.Left.Pressed = btnDPad is 5 or 6 or 7;

        ButtonCross.Pressed = btnA.HasFlag(DualSenseButtonsA.Cross);
        ButtonCircle.Pressed = btnA.HasFlag(DualSenseButtonsA.Circle);
        ButtonSquare.Pressed = btnA.HasFlag(DualSenseButtonsA.Square);
        ButtonTriangle.Pressed = btnA.HasFlag(DualSenseButtonsA.Triangle);

        var btnB = packet.ButtonsB;
        var btnC = packet.ButtonsC;

        LeftShoulder.Pressed = btnB.HasFlag(DualSenseButtonsB.LeftShoulder);
        RightShoulder.Pressed = btnB.HasFlag(DualSenseButtonsB.RightShoulder);
        LeftStick.Pressed = btnB.HasFlag(DualSenseButtonsB.LeftStick);
        RightStick.Pressed = btnB.HasFlag(DualSenseButtonsB.RightStick);
        Share.Pressed = btnB.HasFlag(DualSenseButtonsB.Share);
        Options.Pressed = btnB.HasFlag(DualSenseButtonsB.Options);
        PlayStationLogo.Pressed = btnC.HasFlag(DualSenseButtonsC.Home);
        TouchPad.Pressed = btnC.HasFlag(DualSenseButtonsC.PadButton);
        Mic.Pressed = btnC.HasFlag(DualSenseButtonsC.MicButton);

        var ax = packet.Accelerometer[0] / (float) short.MaxValue;
        var ay = packet.Accelerometer[1] / (float) short.MaxValue;
        var az = packet.Accelerometer[2] / (float) short.MaxValue;
        Accelerometer = new Vector3(ax, ay, az);

        var gx = packet.Gyroscope[0] / (float) short.MaxValue;
        var gy = packet.Gyroscope[1] / (float) short.MaxValue;
        var gz = packet.Gyroscope[2] / (float) short.MaxValue;
        Gyroscope = new Vector3(gx, gy, gz);

        TouchPad.TouchStates[0] = TouchState.FromRaw(packet.Touch1);
        TouchPad.TouchStates[1] = TouchState.FromRaw(packet.Touch2);

        var capacity = (packet.Status & 0b111) * 1f / 0b111;
        var status = (DualSenseBatteryStatus) ((packet.Status & 0b1110000) >> 4);

        var current = new DualSenseSnapshot
        {
            LeftStick = LeftStick.GetState<IControllerStickButton.StickButtonState>(),
            RightStick = RightStick.GetState<IControllerStickButton.StickButtonState>(),
            LeftTrigger = LeftTrigger.Value,
            RightTrigger = RightTrigger.Value,
            ButtonCircle = ButtonCircle.Pressed,
            ButtonCross = ButtonCross.Pressed,
            ButtonSquare = ButtonSquare.Pressed,
            ButtonTriangle = ButtonTriangle.Pressed,
            LeftShoulder = LeftShoulder.Pressed,
            RightShoulder = RightShoulder.Pressed,
            DPad = DPad.GetState(),
            Share = Share.Pressed,
            Options = Options.Pressed,
            TouchPad = TouchPad.State,
            PlayStationLogo = PlayStationLogo.Pressed,
            Mic = Mic.Pressed,
            Accelerometer = Accelerometer,
            Gyroscope = Gyroscope,
            BatteryCapacity = capacity,
            BatteryStatus = status
        };

        LastSnapshot.IfPresent(last =>
        {
            SnapshotUpdated?.Invoke(last, current);
            
            foreach (var button in _buttons)
            {
                TriggerButtonEventIfChanged(button.Button, button.StateFromSnapshot(last),
                    button.StateFromSnapshot(current));
            }
        });

        LastSnapshot = Optional.Of(current);
        return current;
    }

    private void TriggerButtonEventIfChanged(IControllerButton button, bool oldState, bool newState)
    {
        if (oldState == newState) return;
        
        if (newState)
        {
            ButtonPressed?.Invoke(button);
        }
        else
        {
            ButtonReleased?.Invoke(button);
        }
    }

    public unsafe void UpdateStates()
    {
        var f = (byte) (TouchPad.TouchStates[0].Position.X * 255);
        var output = new DualSenseOutputState
        {
            ControlFlags = (DualSenseControlFlags) 0xf7,
            MicLed = (byte) (Mic.IsLedEnabled ? 0x1 : 0x0),
            MicFlag = (byte) (Mic.IsLedEnabled ? 0x10 : 0x00),
            PlayerLed = TouchPad.PlayerLed,
            LightBarColorR = TouchPad.LightBar.R,
            LightBarColorG = TouchPad.LightBar.G,
            LightBarColorB = TouchPad.LightBar.B
        };

        var reportLength = ConnectionType == ConnectionType.Usb ? 63 : 78;
        var buf = new byte[reportLength];

        if (ConnectionType == ConnectionType.Usb)
        {
            buf[0] = 2;
        }
        else
        {
            buf[0] = 0x31;
            buf[1] = (byte) (_sequenceTag << 4);
            buf[2] = 0x10;

            if (++_sequenceTag >= 16)
            {
                _sequenceTag = 0;
            }
        }

        var size = DualSenseOutputState.Size;
        var ptr = Marshal.AllocHGlobal(size);
        for (var i = 0; i < size; i++)
        {
            Marshal.WriteByte(ptr, i, 0);
        }
        
        Marshal.StructureToPtr(output, ptr, false);
        Marshal.Copy(ptr, buf, ConnectionType == ConnectionType.Usb ? 1 : 3, size);
        Marshal.FreeHGlobal(ptr);

        if (ConnectionType == ConnectionType.Bluetooth)
        {
            var report = new byte[buf.Length - 4];
            Array.Copy(buf, report, buf.Length - 4);
            var crc = BitConverter.GetBytes(DoCrc32(report, 0xa2));
            Array.Copy(crc, 0, buf, 74, 4);
        }

        try
        {
            _device.Write(buf);
        }
        catch (HidException)
        {
            Disconnected?.Invoke();
        }
    }

    public static bool IsDualSense(HidDeviceInfo info)
    {
        return info.VendorId == 0x054c && info.ProductId == 0x0ce6;
    }
    
    public static IEnumerable<HidDevice> FindAllDualSense()
    {
        return HidDeviceBrowse.Browse()
            .Where(IsDualSense)
            .Select(info =>
            {
                var device = new HidDevice();
                if (device.Open(info))
                {
                    try
                    {
                        // TODO: Needs a way to test if the connection is alive
                        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            device.Write(new byte[2]);
                        }

                        return device;
                    }
                    catch (HidException)
                    {
                        device.Close();
                        return null;
                    }
                }

                Logger.Error($"Failed to open the device path, path = {info.Path}");
                device.Close();
                return null;
            })
            .Where(n => n != null)
            .Select(n => n!);
    }

    private static uint DoCrc32(byte[] buf, byte seed)
    {
        var crc = Crc32.Shared.Get(new[] { seed });
        crc = ~Crc32.Shared.Get(buf, crc);
        return crc;
    }

    public void Dispose()
    {
        _disposed = true;
        _device.Dispose();
        if (_audioStream != 0)
        {
            Bass.StreamFree(_audioStream);
            Bass.CurrentDevice = _audioDeviceId;
            Bass.Free();
        }
    }
}