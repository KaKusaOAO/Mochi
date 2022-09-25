using System.Numerics;
using System.Runtime.InteropServices;
using KaLib.IO.Hid;
using KaLib.IO.Hid.Native;
using KaLib.Utils;
using KaLib.Utils.Extensions;

namespace KaLib.IO.Controllers.DualSense;

public class DualSenseController : IController<DualSenseSnapshot>, IHybridController
{
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
    
    /// <summary>
    /// The accelerometer values in G.
    /// </summary>
    public Vector3 Accelerometer { get; private set; }
    
    /// <summary>
    /// The gyroscope values in deg/s.
    /// </summary>
    public Vector3 Gyroscope { get; private set; }

    private const int AccResolutionPerG = 8192;
    private const int AccRange = 4 * AccResolutionPerG;

    private const int GyroResolutionPerDegS = 1024;
    private const int GyroRange = 2048 * GyroResolutionPerDegS;

    public event Action<IControllerButton> ButtonPressed;
    public event Action<IControllerButton> ButtonReleased;
    public event Action<DualSenseSnapshot, DualSenseSnapshot> SnapshotUpdated;
    public event Action Disconnected;

    private IOptional<DualSenseSnapshot> _lastSnapshot = Optional.Empty<DualSenseSnapshot>();
    private byte _sequenceTag;

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

    private record ButtonDescription(IControllerButton Button, Func<DualSenseSnapshot, bool> StateFromSnapshot);

    public unsafe DualSenseSnapshot PollInput()
    {
        var size = ConnectionType == ConnectionType.Usb ? 64 : 78;
        var buf = new byte[size];
        try
        {
            var read = _device.ReadTimeout(buf, 64);
            if (read == 0) throw new Exception("Read timed out");
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
        LeftStick.Vector = new Vector2(x, y);

        x = packet.RightStickX / 128f - 1;
        y = packet.RightStickY / 128f - 1;
        RightStick.Vector = new Vector2(x, y);

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

        _lastSnapshot.IfPresent(last =>
        {
            SnapshotUpdated?.Invoke(last, current);
            
            foreach (var button in _buttons)
            {
                TriggerButtonEventIfChanged(button.Button, button.StateFromSnapshot(last),
                    button.StateFromSnapshot(current));
            }
        });

        _lastSnapshot = Optional.Of(current);
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
            Flag1 = 0xff,
            ControlFlags = (DualSenseControlFlags) 0xf7,
            LeftRumble = 0,
            RightRumble = 0,
            MicLed = (byte) (Mic.IsLedEnabled ? 0x1 : 0x0),
            MicFlag = (byte) (Mic.IsLedEnabled ? 0x10 : 0x00),
            RightAdaptiveTriggerState = new AdaptiveTriggerState
            {
                Mode = AdaptiveTriggerMode.Section,
                Forces = new AdaptiveTriggerForces
                {
                    Force1 = f
                }
            },
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

    public static HidDevice? FindFirstDualSense()
    {
        var info = HidDeviceBrowse.Browse().Find(IsDualSense);
        if (info == null) return null;

        var device = new HidDevice();
        if (device.Open(info)) return device;

        Logger.Error($"Failed to open the device path, path = {info.Path}");
        device.Close();
        return null;
    }

    private static uint DoCrc32(byte[] buf, byte seed)
    {
        var crc = Crc32.Shared.Get(new[] { seed });
        crc = ~Crc32.Shared.Get(buf, crc);
        return crc;
    }

    public void Dispose()
    {
        _device.Dispose();
    }
}